using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Victoria;
using Victoria.Enums;
using Victoria.Responses.Search;

namespace NamazuKingdom.Modules
{

    public sealed class AudioModule : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;
        public AudioModule(LavaNode node)
        {
            _lavaNode = node;
        }
        [Command("join")]
        public async Task JoinAsync()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm already connected to a voice channel!");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("leave")]
        public async Task LeaveAsync()
        {
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel!");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            try
            {
                await _lavaNode.LeaveAsync(voiceState.VoiceChannel);
                await ReplyAsync($"Left {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
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
            foreach(var sound in sounds)
            {
                var s = sound.Substring(0, sound.IndexOf('.'));
                soundListStr += $"{(i++)}: {s}\n";
            }
            await Context.Channel.SendMessageAsync(soundListStr);
        }

        [Command("play")]
        public async Task PlayAsync([Remainder][Summary("The sound to play")] string sound)
        {
            if (string.IsNullOrWhiteSpace(sound))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (!sound.Contains("http"))
                sound = "sounds/" + sound + ".mp3";
            var searchResponse = await _lavaNode.SearchAsync(SearchType.Direct, sound);
            if (searchResponse.Status is SearchStatus.LoadFailed or SearchStatus.NoMatches)
            {
                await ReplyAsync($"I wasn't able to find anything for `{sound}`.");
                return;
            }
            var player = _lavaNode.GetPlayer(Context.Guild);
            if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
            {
                player.Queue.Enqueue(searchResponse.Tracks);
                await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} songs.");
            }
            else
            {
                var track = searchResponse.Tracks.FirstOrDefault();
                player.Queue.Enqueue(track);

                await ReplyAsync($"Enqueued {track?.Id}");
            }

            if (player.PlayerState is PlayerState.Playing or PlayerState.Paused)
            {
                return;
            }

            player.Queue.TryDequeue(out var lavaTrack);
            await player.PlayAsync(x => {
                x.Track = lavaTrack;
                x.ShouldPause = false;
            });
        }

    }
}
