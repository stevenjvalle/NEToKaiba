using Discord;
using Discord.Interactions; 
using Microsoft.EntityFrameworkCore;
using NamazuKingdom.Helpers;
using NamazuKingdom.Models;
using NamazuKingdom.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamazuKingdom.Modules
{
    [Group("admin", "administrators")]
    public class AdminModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DatabaseServices _databaseServices;
        public AdminModule(DatabaseServices databaseServices)
        {
            _databaseServices = databaseServices;
        }

        [SlashCommand("add", "Add user to database.")]
        public async Task AddAsync(IGuildUser user, string nickname, string pronouns, string birthday, string useTTS, string ttsVoiceName,
            string showBirthday, string showNickname, string showPronouns, string shouldDisableSounds, string soundLevel)
        {
            DateTime birthdayFormatted = new();
            DateTime.TryParse(birthday, out birthdayFormatted);
            bool useTTSFormatted = false;
            bool.TryParse(useTTS, out useTTSFormatted);
            bool showBirthdayFormatted = false;
            bool.TryParse(showBirthday, out showBirthdayFormatted);
            bool showNicknameFormatted = false;
            bool.TryParse(showNickname, out showNicknameFormatted);
            bool showPronounsFormatted = false;
            bool.TryParse(showPronouns, out showPronounsFormatted);
            bool shouldDisableSoundsFormatted = true;
            bool.TryParse(shouldDisableSounds, out shouldDisableSoundsFormatted);
            int soundLevelFormatted = 100;
            int.TryParse(soundLevel, out soundLevelFormatted);
            await _databaseServices.CreateUser(user.Id, nickname, pronouns, birthdayFormatted, useTTSFormatted, ttsVoiceName,
                showBirthdayFormatted, showNicknameFormatted ,showPronounsFormatted, shouldDisableSoundsFormatted, soundLevelFormatted);
        }
        [SlashCommand("show_info", "Print details on a user from database.")]
        public async Task ShowInfoAsync(IGuildUser user)
        {
            var dUser = await _databaseServices.FindUser(user.Id);
            if (dUser == null)
            { 
                await ReplyAsync($"Could not find user with ID {user.Id}, was this user even added?");
                return;
            }
            await ReplyAsync($"Hello {dUser.Nickname}, your birthday will be on {dUser.Birthday.ToString("d")}");
        }
    }
}
