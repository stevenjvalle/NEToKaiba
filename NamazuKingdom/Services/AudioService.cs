using Discord.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamazuKingdom.Services
{
    public class AudioService
    {
        // We want to keep our audio client alive for as long as the bot is connected to a vc
        public IAudioClient? AudioClient { get; set; }
        //We need to keep the AudioOutStream alive because of https://stackoverflow.com/questions/53917665/audio-output-from-memorystream-using-tts-to-discord-bot
        public AudioOutStream? AudioOutStream { get; set; }
        public readonly WaveFormat WaveFormat = new WaveFormat(48000, 16, 2);

    }
}
