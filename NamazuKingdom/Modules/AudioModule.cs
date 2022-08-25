using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualBasic;
using NamazuKingdom.Helpers;
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
        private DiscordSocketClient _client;

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

            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            try
            {
                var audioClient = await channel.ConnectAsync();
                _audioService.AudioClient = audioClient;
                _audioService.AudioOutStream = audioClient.CreatePCMStream(AudioApplication.Mixed);
                await PlaySilence();
                //await _audioService.AudioOutStream.FlushAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
        //There is some very strange issue where short clips won't play when they are the first clip
        //to be played. So We are now cheating and playing silence as the first stream which should 
        //allow all other clips to be played. 
        private async Task PlaySilence()
        {
            int bytesPerMillisecond = _audioService.WaveFormat.AverageBytesPerSecond / 1000;
            //an new all zero byte array will play silence
            var silentBytes = new byte[1000 * bytesPerMillisecond];
            MemoryStream stream = new(silentBytes);
            var reader = new RawSourceWaveStream(stream, _audioService.WaveFormat);
            using var resamplerDmo = new ResamplerDmoStream(reader, _audioService.WaveFormat);
            try
            {
                await resamplerDmo.CopyToAsync(_audioService.AudioOutStream);
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            finally
            {
                await _audioService.AudioOutStream.FlushAsync();
            }
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
            if(_audioService.AudioClient != null)
            {
                await _audioService.AudioClient.StopAsync();
                _audioService.AudioClient = null;
            }
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
            await SendAsync(sound);
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
            await SendAsync(url);
        }
        private async Task SendAsync(string path)
        {
            try
            {
                using (var reader = new MediaFoundationReader(path))
                {
                    using var resamplerDmo = new ResamplerDmoStream(reader, _audioService.WaveFormat);
                    try
                    {
                        await resamplerDmo.CopyToAsync(_audioService.AudioOutStream);
                    }
                    catch (Exception ex) { Console.WriteLine(ex); }
                    finally { 
                        await _audioService.AudioOutStream.FlushAsync(); 
                    }   
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
