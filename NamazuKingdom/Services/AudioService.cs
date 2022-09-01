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
        private readonly WaveFormat _waveFormat = new WaveFormat(48000, 16, 2);
        private readonly List<GuildAudioService> _audioServiceList;
        public AudioService(List<GuildAudioService> audioServiceList)
        {
            _audioServiceList = audioServiceList;
        }
        public void CreateAudioService(IVoiceChannel channel, ulong guildID, IAudioClient client)
        {
            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            try
            {
                //Adding a smaller buffer seems to fix the issue where when short clips are played
                //as the first sound it will break the stream. (https://github.com/discord-net/Discord.Net/issues/687)
                var audioOutStream = client.CreatePCMStream(AudioApplication.Mixed, 48000, 200);
                var guildAudioService = new GuildAudioService(channel, client, audioOutStream, guildID);
                _audioServiceList.Add(guildAudioService);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool IsConnected(ulong guildID)
        {
            var guildAudioService = _audioServiceList.Where(s => s.GuildID == guildID).First();
            if(guildAudioService == null)
            {
                //TODO
                Console.WriteLine("Is not connected");
                return false;
            }
            try
            {
                if (guildAudioService.AudioClient != null && guildAudioService.AudioOutStream!.CanWrite)
                    return true;
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public IVoiceChannel? GetCurrentVoiceChannel(ulong guildID)
        {
            var guildAudioService = _audioServiceList.Where(s => s.GuildID == guildID).First();
            if (guildAudioService == null)
            {
                //TODO
                Console.WriteLine("Is not connected");
                return null;
            }
            return guildAudioService.GuildChannel;
        }

        public async Task DestroyAudioService(ulong guildID)
        {
            var guildAudioService = _audioServiceList.Where(s => s.GuildID == guildID).First();
            if (guildAudioService == null)
            {
                //TODO
                Console.WriteLine("Is not connected");
                return;
            }
            if (guildAudioService.AudioClient != null)
            {
                await guildAudioService.AudioClient.StopAsync();
                guildAudioService.AudioOutStream?.Dispose();
                guildAudioService.AudioClient = null;
                _audioServiceList.Remove(guildAudioService);
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
        public async Task SendAsync(string path, ulong guildID)
        {
            var guildAudioService = _audioServiceList.Where(s => s.GuildID == guildID).First();
            if (guildAudioService == null)
            {
                //TODO
                Console.WriteLine("Is not connected");
                return;
            }
            if (guildAudioService.AudioClient == null || guildAudioService.AudioOutStream == null)
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
                    await resampler.CopyToAsync(guildAudioService.AudioOutStream);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
                finally
                {
                    await guildAudioService.AudioOutStream.FlushAsync();
                }
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
    public class GuildAudioService
    {
        public IVoiceChannel GuildChannel { get; set; }
        public IAudioClient? AudioClient { get; set; }
        public AudioOutStream? AudioOutStream { get; set; }
        public ulong GuildID { get; set; }
        public GuildAudioService(IVoiceChannel channel, IAudioClient? audioClient, AudioOutStream? audioOutStream, ulong guildID)
        {
            GuildChannel = channel;
            AudioClient = audioClient;
            AudioOutStream = audioOutStream;
            GuildID = guildID;
        }
    }
}
