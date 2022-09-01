using Discord;
using Discord.Interactions; 
using Microsoft.EntityFrameworkCore;
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
        private NamazuKingdomDbContext _dbContext;
        public AdminModule(NamazuKingdomDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [SlashCommand("add", "Add user to database.")]
        public async Task AddAsync(IGuildUser user)
        {
            DiscordUsers dUser = new DiscordUsers();
            UserSettings settings = new UserSettings();
            try
            {
                await _dbContext.DiscordUsers.AddAsync(dUser);
                await _dbContext.UserSettings.AddAsync(settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            dUser.DiscordUserId = user.Id;
            dUser.PreferredPronouns = "They/Them";
            dUser.Birthday = new DateTime(2000, 01, 01);
            dUser.UserSettings = settings;
            
            settings.DiscordUser = dUser;
            settings.UserRefId = dUser.Id;
            settings.TTSVoiceName = "Amy";
            settings.UseTTS = true;
            settings.ShouldShowBirthday = true;
            settings.ShouldDisableSounds = false;
            settings.ShouldShowBirthday = false;
            settings.ShouldShowNickname = false;
            settings.ShouldShowPronouns = false;
            try
            {

                await _dbContext.SaveChangesAsync();
                await ReplyAsync($"Added user {user.Id}");
                Console.WriteLine($"Added user {user.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        [SlashCommand("show_info", "Print details on a user from database.")]
        public async Task ShowInfoAsync(IGuildUser user)
        {
            var dUser = await _dbContext.DiscordUsers.FirstOrDefaultAsync(u => u.DiscordUserId == user.Id);
            if (dUser == null)
            { 
                await ReplyAsync($"Could not find user with ID {user.Id}, was this user even added?");
                return;
            }
            await ReplyAsync($"Hello {dUser.Nickname}, your birthday will be on {dUser.Birthday.ToString("d")}");
        }
    }
}
