using Discord.Audio;
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
        public AudioService()
        {
        }
    }
}
