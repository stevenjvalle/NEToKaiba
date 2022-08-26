using Discord;
using Discord.Commands;
using NamazuKingdom.Models;
using NamazuKingdom.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamazuKingdom.Modules
{
    [Group("admin")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        private NamazuKingdomDbContext _dbContext;
        public AdminModule(NamazuKingdomDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Command("add")]
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
            settings.TTSVoiceName = "Brian";
            settings.UseTTS = true;
            settings.ShouldShowBirthday = true;
            try
            {

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
