using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualBasic;
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

        public AudioModule(AudioService audioService, DiscordSocketClient client)
        {
            _audioService = audioService;
        }

        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinAsync(IVoiceChannel channel = null)
        {
            // Get the audio channel
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }
            var audioClient = await channel.ConnectAsync();
            _audioService.CreateAudioService(audioClient);
        }


        [Command("list_sounds")]
        public async Task ListSoundsAsync()
        {
            //todo: add to appsettings
            var soundsFolder = "Lavalink\\sounds";
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
            var url = "https://api.streamelements.com/kappa/v2/speech?voice=Amy&text="+sound;
            await _audioService.SendAsync(url);
        }

    }
}
