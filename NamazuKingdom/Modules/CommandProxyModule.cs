using Discord;
using Discord.Commands;
using NamazuKingdom.Helpers;
using NamazuKingdom.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamazuKingdom.Modules
{
    // Class to execute TTS as a command still. 
    public class CommandProxyModule : ModuleBase<SocketCommandContext>
    {

        private readonly AudioService _audioService;
        private readonly DatabaseServices _databaseServices;
        public CommandProxyModule(AudioService audioService, DatabaseServices dbServices)
        {
            _audioService = audioService;
            _databaseServices = dbServices;
        }

        [Command("tts")]
        public async Task TTSAsync([Remainder] string sound)
        {
            Console.WriteLine("TTS Started:");
            Console.WriteLine(sound);
            //todo null handle
            if (Context.User == null) return;

            var botVoice = await _databaseServices.UserTTSVoice(Context.User.Id);

            if (string.IsNullOrWhiteSpace(sound))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }
            Console.WriteLine("Pass null or white space test. ");
            Console.WriteLine("Guild ID:");
            Console.WriteLine(Context.Guild.Id);
            if (_audioService.IsConnected(Context.Guild.Id) == false)
            {
                await ReplyAsync("Audio client is null, am I connected to a voice channel?");
                return;
            }
            Console.WriteLine("Pass Guild Connection test. ");
            if (string.IsNullOrWhiteSpace(sound))
                return;
            if (sound.Contains("https://") || sound.Contains("http://"))
                return;
            Console.WriteLine("Pass URL. ");
            if ((Context.User as IGuildUser)?.VoiceChannel == null ||
                (Context.User as IGuildUser)?.VoiceChannel != _audioService.GetCurrentVoiceChannel(Context.Guild.Id))
            {
                await ReplyAsync("You are not connected to the same voice channel as the bot.");
                return;
            }
            Console.WriteLine("Test this");
            var url = $"https://api.streamelements.com/kappa/v2/speech?voice={botVoice}&text={StringHelpers.CleanTTSString(sound)}";//+StringHelpers.CleanTTSString(sound);
            Console.WriteLine(url);
            await _audioService.SendAsync(url, Context.Guild.Id);
        }
    }
}
