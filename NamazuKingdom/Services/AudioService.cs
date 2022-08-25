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
        public IAudioClient? AudioClient { get; set; }
        public AudioOutStream AudioOutStream { get; set; }
        public readonly WaveFormat WaveFormat = new WaveFormat(48000, 16, 2);
        public AudioService()
        {
        }
    }
}
