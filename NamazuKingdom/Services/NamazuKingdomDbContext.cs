using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NamazuKingdom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamazuKingdom.Services
{
    public class NamazuKingdomDbContext : DbContext
    {
        public NamazuKingdomDbContext(DbContextOptions<NamazuKingdomDbContext> options)
            : base(options)
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source=bot.db");

        public DbSet<DiscordUsers> DiscordUsers { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<DiscordUsers>()
                .HasOne(a => a.UserSettings)
                .WithOne(b => b.DiscordUser)
                .HasForeignKey<UserSettings>(b => b.UserRefId);
        }
    }
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<NamazuKingdomDbContext>
    {
        public NamazuKingdomDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var builder = new DbContextOptionsBuilder<NamazuKingdomDbContext>();
            
            builder.UseSqlite($"Data Source=bot.db");
            return new NamazuKingdomDbContext(builder.Options);
        }
    }
}
