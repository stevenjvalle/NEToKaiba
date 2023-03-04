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
        private readonly AudioService _audioService;
        private readonly DatabaseServices _databaseServices;
        public AudioModule(AudioService audioService, DatabaseServices dbServices)
        {
            _audioService = audioService;
            _databaseServices = dbServices;
        }

        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinAsync(IVoiceChannel channel = null)
        {
            // Get the audio channel
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

            Console.WriteLine($"Channel: {channel.Id}");
            var audioClient = await channel.ConnectAsync();
            Console.WriteLine($"Audio Client: {audioClient.ConnectionState}");
            _audioService.CreateAudioService(channel, channel.GuildId, audioClient);
            Console.WriteLine($"Is connected: {_audioService.IsConnected(channel.GuildId)}");
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
            await _audioService.DestroyAudioService(Context.Guild.Id);
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
            if(_audioService.IsConnected(Context.Guild.Id) == false)
            {
                await ReplyAsync("Audio client is null, am I connected to a voice channel?");
                return;
            }
            if((Context.User as IGuildUser)?.VoiceChannel == null ||
                (Context.User as IGuildUser)?.VoiceChannel != _audioService.GetCurrentVoiceChannel(Context.Guild.Id))
            {
                await ReplyAsync("You are not connected to the same voice channel as the bot.");
                return;
            }
            await _audioService.SendAsync(sound, Context.Guild.Id);
        }

        [Command("tts")]
        public async Task TTSAsync([Remainder][Summary("The sound to play")] string sound)
        {
            //todo null handle
            if (Context.User == null) return;

            var botVoice = await _databaseServices.UserTTSVoice(Context.User.Id);

            if (string.IsNullOrWhiteSpace(sound))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }
            if (_audioService.IsConnected(Context.Guild.Id) == false)
            {
                await ReplyAsync("Audio client is null, am I connected to a voice channel?");
                return;
            }
            if (string.IsNullOrWhiteSpace(sound))
                return;
            if (sound.Contains("https://") || sound.Contains("http://"))
                return;
            if ((Context.User as IGuildUser)?.VoiceChannel == null ||
                (Context.User as IGuildUser)?.VoiceChannel != _audioService.GetCurrentVoiceChannel(Context.Guild.Id))
            {
                await ReplyAsync("You are not connected to the same voice channel as the bot.");
                return;
            }
            var url = $"https://api.streamelements.com/kappa/v2/speech?voice={botVoice}&text={StringHelpers.CleanTTSString(sound)}";//+StringHelpers.CleanTTSString(sound);
            await _audioService.SendAsync(url, Context.Guild.Id);
        }

    }
}
