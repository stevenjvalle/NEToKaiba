using Discord;
using Discord.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NamazuKingdom.Services
{
    public class AudioService
    {
        // We want to keep our audio client alive for as long as the bot is connected to a vc
        public IAudioClient? AudioClient { get; set; }
        //We need to keep the AudioOutStream alive because of https://stackoverflow.com/questions/53917665/audio-output-from-memorystream-using-tts-to-discord-bot
        private AudioOutStream? _audioOutStream { get; set; }
        private readonly WaveFormat _waveFormat = new WaveFormat(48000, 16, 2);

        public void CreateAudioService(IAudioClient client)
        {

            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            try
            {
                AudioClient = client;
                //Adding a smaller buffer seems to fix the issue where when short clips are played
                //as the first sound it will break the stream. (https://github.com/discord-net/Discord.Net/issues/687)
                _audioOutStream = client.CreatePCMStream(AudioApplication.Mixed, 48000, 200);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task DestroyAudioService()
        {
            if (AudioClient != null)
            {
                await AudioClient.StopAsync();
                _audioOutStream?.Dispose();
                AudioClient = null;
            }
        }

        private async Task<ResamplerDmoStream> CreateResamplerFromStream(string path)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(path);
                var contentStream = await response.Content.ReadAsStreamAsync();
                var reader = new Mp3FileReader(contentStream);
                return new ResamplerDmoStream(reader, _waveFormat);
            }
        }
        private ResamplerDmoStream CreateResamplerFromFile(string path)
        {
            using (var reader = new MediaFoundationReader(path))
            {
                return new ResamplerDmoStream(reader, _waveFormat);
            }
        }
        public async Task SendAsync(string path)
        {
            if (AudioClient == null || _audioOutStream == null)
            {
                Console.WriteLine("Client or stream is null");
                return;
            }
            try
            {
                ResamplerDmoStream? resampler = null;
                if (path.Contains("https://"))
                {
                    resampler = await CreateResamplerFromStream(path);
                }
                else
                {
                    resampler = CreateResamplerFromFile(path);
                }
                try
                {
                    await resampler.CopyToAsync(_audioOutStream);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
                finally
                {
                    await _audioOutStream.FlushAsync();
                }
                /*using (var reader = new MediaFoundationReader(path))
                {
                    using var resamplerDmo = new ResamplerDmoStream(reader, _waveFormat);
                    try
                    {
                        await resamplerDmo.CopyToAsync(_audioOutStream);
                    }
                    catch (Exception ex) { Console.WriteLine(ex); }
                    finally
                    {
                        await _audioOutStream.FlushAsync();
                    }
                }*/
                /*ResamplerDmoStream? resamplerDmo = null;
                if (path.Contains("https://"))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var response = await client.GetAsync(path);
                        var contentStream = await response.Content.ReadAsStreamAsync();
                        using (var reader = new Mp3FileReader(contentStream))
                        {
                            resamplerDmo = new ResamplerDmoStream(reader, _waveFormat);
                        }
                    }
                }
                else
                {
                    using (var reader = new MediaFoundationReader(path))
                    {
                        resamplerDmo = new ResamplerDmoStream(reader, _waveFormat);
                    }
                }
                try
                {
                    await resamplerDmo.CopyToAsync(_audioOutStream);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
                finally
                {
                    await _audioOutStream.FlushAsync();
                }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        //OUTDATED FIX, HOWEVER WE ARE KEEPING THIS JUST SO WE KNOW HOW TO CREATE AUDIO STREAMS FOR NAUDIO
        //There is some very strange issue where short clips won't play when they are the first clip
        //to be played. So We are now cheating and playing silence as the first stream which should 
        //allow all other clips to be played. 
        /*private async Task PlaySilence()
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
        }*/
    }
}
