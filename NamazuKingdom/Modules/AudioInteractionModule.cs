using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Interactions; 
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
    public sealed class AudioInteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        // private readonly AudioService _audioService;
        // private readonly DatabaseServices _databaseServices;
        // public AudioInteractionModule(AudioService audioService, DatabaseServices dbServices)
        // {
        //     _audioService = audioService;
        //     _databaseServices = dbServices;
        // }

        // [SlashCommand("join", "Have bot join the channel.")]
        // public async Task JoinAsync(IVoiceChannel channel = null)
        // {
        //     // Get the audio channel
        //     Console.WriteLine("Join Set"); 
        //     channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
        //     if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

        //     Console.WriteLine($"Channel: {channel.Id}");
        //     var audioClient = await channel.ConnectAsync();
        //     Console.WriteLine($"Audio Client: {audioClient.ConnectionState}");
        //     _audioService.CreateAudioService(channel, channel.GuildId, audioClient);
        //     Console.WriteLine($"Is connected: {_audioService.IsConnected(channel.GuildId)}");
        // }

        // [SlashCommand("list_sounds", "List Available Sounds")]
        // public async Task ListSoundsAsync()
        // {
        //     //todo: add to appsettings
        //     var soundsFolder = "sounds";
        //     try
        //     {
        //         var sounds = new DirectoryInfo(soundsFolder).GetFiles().Select(o => o.Name).ToArray();
        //         var soundListStr = "List of Sounds\n================\n";
        //         var i = 1;
        //         foreach (var sound in sounds)
        //         {
        //             var s = sound.Substring(0, sound.IndexOf('.'));
        //             soundListStr += $"{(i++)}: {s}\n";
        //         }
        //         await Context.Channel.SendMessageAsync(soundListStr);
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex);
        //     }
        // }

        // [SlashCommand("leave", "Have bot leave the active channel.")]
        // public async Task LeaveAsync()
        // {
        //     await _audioService.DestroyAudioService(Context.Guild.Id);
        //     await ReplyAsync("Good-bye!");
        // }

        // [SlashCommand("play", "Plays a linked audio cue. ")]
        // public async Task PlayAsync([Remainder] string sound)
        // {
        //     if (string.IsNullOrWhiteSpace(sound))
        //     {
        //         await ReplyAsync("Please provide search terms.");
        //         return;
        //     }
        //     sound = "sounds/" + sound + ".mp3";
        //     if(_audioService.IsConnected(Context.Guild.Id) == false)
        //     {
        //         await ReplyAsync("Audio client is null, am I connected to a voice channel?");
        //         return;
        //     }
        //     if((Context.User as IGuildUser)?.VoiceChannel == null ||
        //         (Context.User as IGuildUser)?.VoiceChannel != _audioService.GetCurrentVoiceChannel(Context.Guild.Id))
        //     {
        //         await ReplyAsync("You are not connected to the same voice channel as the bot.");
        //         return;
        //     }
        //     await _audioService.SendAsync(sound, Context.Guild.Id);
        // }

        // [SlashCommand("tts", "Invoke the Text-To-Speech of a connected bot as a non-subscriber. ")]
        // public async Task TTSAsync([Remainder] string sound)
        // {
        //     //todo null handle
        //     if (Context.User == null) return;

        //     var botVoice = await _databaseServices.UserTTSVoice(Context.User.Id);

        //     if (string.IsNullOrWhiteSpace(sound))
        //     {
        //         await ReplyAsync("Please provide search terms.");
        //         return;
        //     }
        //     if (_audioService.IsConnected(Context.Guild.Id) == false)
        //     {
        //         await ReplyAsync("Audio client is null, am I connected to a voice channel?");
        //         return;
        //     }
        //     if (string.IsNullOrWhiteSpace(sound))
        //         return;
        //     if (sound.Contains("https://") || sound.Contains("http://"))
        //         return;
        //     if ((Context.User as IGuildUser)?.VoiceChannel == null ||
        //         (Context.User as IGuildUser)?.VoiceChannel != _audioService.GetCurrentVoiceChannel(Context.Guild.Id))
        //     {
        //         await ReplyAsync("You are not connected to the same voice channel as the bot.");
        //         return;
        //     }
        //     var url = $"https://api.streamelements.com/kappa/v2/speech?voice={botVoice}&text={StringHelpers.CleanTTSString(sound)}";//+StringHelpers.CleanTTSString(sound);
        //     await _audioService.SendAsync(url, Context.Guild.Id);
        // }

    }
}
