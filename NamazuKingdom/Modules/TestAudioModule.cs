using Discord;
using Discord.Audio;
using Discord.Commands;
using NamazuKingdom.Services;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NamazuKingdom.Modules
{
    public sealed class TestAudioModule : ModuleBase<SocketCommandContext>
    {
        private AudioService _audioService;

        public TestAudioModule(AudioService audioService)
        {
            _audioService = audioService;
        }

        [Command("test-join", RunMode = RunMode.Async)]
        public async Task TestJoinAsync(IVoiceChannel channel = null)
        {
            // Get the audio channel
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            try
            {
                var audioClient = await channel.ConnectAsync();
                _audioService.AudioClient = audioClient;
                _audioService.AudioOutStream = audioClient.CreatePCMStream(AudioApplication.Music);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("test-play")]
        public async Task TestPlayAsync([Remainder][Summary("The sound to play")] string sound)
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
            await SendAsync(_audioService.AudioClient, sound);
        }
        private async Task SendAsync(IAudioClient client, string path)
        {
            try
            {
                using (var reader = new MediaFoundationReader(path))
                {
                    var format = new WaveFormat(48000, 16, 2);
                    using var resamplerDmo = new ResamplerDmoStream(reader, format);
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
