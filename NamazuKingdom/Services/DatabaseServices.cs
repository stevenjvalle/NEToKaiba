using Discord;
using Microsoft.EntityFrameworkCore;
using NamazuKingdom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamazuKingdom.Services
{
    public class DatabaseServices
    {
        private readonly NamazuKingdomDbContext _dbContext;
        public DatabaseServices()
        {
            _dbContext = new NamazuKingdomDbContext(new DbContextOptions<NamazuKingdomDbContext>());
        }

        public void EnsureCreated()
        {
            _dbContext.Database.EnsureCreated();
        }

        public async Task<DiscordUsers?> FindUser(ulong userId)
        {
            return await _dbContext.DiscordUsers.Where(u => u.DiscordUserId == userId).FirstOrDefaultAsync();
        }

        //todo better way of finding settings
        public async Task<bool> UserWantsTTS(ulong userId)
        {
            var userWantsTTS = _dbContext.UserSettings.
                Where(us => us.DiscordUser.DiscordUserId == userId && us.UseTTS == true)
                .FirstOrDefaultAsync() != null ? true : false;
            return userWantsTTS;
        }
        //really need a better way to get user settings :^), just trying to hit parity 
        public async Task<string> UserTTSVoice(ulong userId)
        {
            var userVoice = "Brian";
            var userVoiceDb = await _dbContext.UserSettings.Where(u => u.DiscordUser.DiscordUserId == userId).FirstOrDefaultAsync();
            if (userVoiceDb != null && !string.IsNullOrEmpty(userVoiceDb.TTSVoiceName))
                userVoice = userVoiceDb.TTSVoiceName;
            return userVoice;
        }

        public async Task<DiscordUsers?> CreateUser(ulong userId, string nickname = "", string pronouns = "", DateTime birthday = new(),
            bool useTTS = false, string ttsVoiceName = "Brian", bool showBirthday = false, bool showNickname = false, bool showPronouns = false, 
            bool shouldDisableSounds = true, int soundLevel = 100)
        {
            try
            {
                var foundUser = await _dbContext.DiscordUsers.Where(u => u.DiscordUserId == userId).FirstOrDefaultAsync();
                if (foundUser != null)
                    return foundUser;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            DiscordUsers dUser = new DiscordUsers();
            UserSettings settings = new UserSettings();

            dUser.DiscordUserId = userId;
            dUser.PreferredPronouns = pronouns;
            dUser.Birthday = birthday;
            dUser.UserSettings = settings;

            settings.DiscordUser = dUser;
            settings.UserRefId = dUser.Id;
            settings.TTSVoiceName = ttsVoiceName;
            settings.UseTTS = useTTS;
            settings.ShouldShowBirthday = showBirthday;
            settings.ShouldDisableSounds = shouldDisableSounds;
            settings.ShouldShowBirthday = showBirthday;
            settings.ShouldShowNickname = showNickname;
            settings.ShouldShowPronouns = showPronouns;
            try
            {
                await _dbContext.DiscordUsers.AddAsync(dUser);
                await _dbContext.UserSettings.AddAsync(settings);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            return dUser;
        }
    }
}
