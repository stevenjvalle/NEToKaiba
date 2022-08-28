using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NamazuKingdom.Helpers;
using NamazuKingdom.Models;
using NamazuKingdom.Services;
using NAudio.MediaFoundation;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace NamazuKingdom.Modules
{
    public sealed class AudioModule : ModuleBase<SocketCommandContext>
    {
        private AudioService _audioService;
        private NamazuKingdomDbContext _dbContext;
        public AudioModule(AudioService audioService, NamazuKingdomDbContext dbContext)
        {
            _audioService = audioService;
            _dbContext = dbContext;
        }

        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinAsync(IVoiceChannel channel = null)
        {
            // Get the audio channel
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            Console.WriteLine($"Channel: {channel.Id}");
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }
            var audioClient = await channel.ConnectAsync();
            Console.WriteLine($"Audio Client: {audioClient.ConnectionState}");
            _audioService.CreateAudioService(audioClient);
            Console.WriteLine($"Is connected: {_audioService.IsConnected()}");
        }


        [Command("list_sounds")]
        public async Task ListSoundsAsync()
        {
            //todo: add to appsettings
            var soundsFolder = "sounds";
            try
            {
                var sounds = new DirectoryInfo(soundsFolder).GetFiles().Select(o => o.Name).ToArray();
                var soundListStr = "List of Sounds\n================\n";
                var i = 1;
                foreach (var sound in sounds)
                {
                    var s = sound.Substring(0, sound.IndexOf('.'));
                    soundListStr += $"{(i++)}: {s}\n";
                }
                await Context.Channel.SendMessageAsync(soundListStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [Command("leave")]
        public async Task LeaveAsync()
        {
            await _audioService.DestroyAudioService();
            await ReplyAsync("Good-bye!");
        }

        [Command("play")]
        public async Task PlayAsync([Remainder][Summary("The sound to play")] string sound)
        {
            if (string.IsNullOrWhiteSpace(sound))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }
            sound = "sounds/" + sound + ".mp3";
            if(_audioService.AudioClient == null)
            {
                await ReplyAsync("Audio client is null, am I connected to a voice channel?");
                return;
            }
            await _audioService.SendAsync(sound);
        }

        [Command("tts")]
        public async Task TTSAsync([Remainder][Summary("The sound to play")] string sound)
        {
            //todo null handle
            if (_dbContext == null) return;
            if (_dbContext.UserSettings == null) return;
            if (Context.User == null) return;

            var botVoice = (await _dbContext.UserSettings.FirstOrDefaultAsync(u => u.DiscordUser.DiscordUserId == Context.User.Id))
                .TTSVoiceName;
            
            if (string.IsNullOrWhiteSpace(sound))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }
            if (_audioService.AudioClient == null)
            {
                await ReplyAsync("Audio client is null, am I connected to a voice channel?");
                return;
            }
            if (string.IsNullOrWhiteSpace(sound))
                return;
            if (sound.Contains("https://") || sound.Contains("http://"))
                return;
            var url = $"https://api.streamelements.com/kappa/v2/speech?voice={botVoice}&text="+StringHelpers.CleanTTSString(sound);
            await _audioService.SendAsync(url);
        }

    }
}
