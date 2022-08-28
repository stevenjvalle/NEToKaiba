﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NamazuKingdom.Services;
using System;

namespace NamazuKingdom // Note: actual namespace depends on the project name.
{
    public class Program
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public Program()
        {
            _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "DC_")
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
            _serviceProvider = CreateProvider(_configuration);
            initiateSlashCommands(); 
        }

        static void Main(string[] args)
            => new Program().RunAsync(args).GetAwaiter().GetResult();

        static IServiceProvider CreateProvider(IConfiguration config)
        {
            var socketConfig = new DiscordSocketConfig();
            var collection = new ServiceCollection()
                .AddSingleton(socketConfig)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton(config)
                .AddSingleton<AudioService>()
                .AddDbContext<NamazuKingdomDbContext>();
            return collection.BuildServiceProvider();
        }

        void initiateSlashCommands(){
            var guildId =  _configuration["guildId"]; 
            
        }

        async Task RunAsync(string[] args)
        {
            //Uncomment if you want the bat file to be launched within the command prompt 
            //StartLavalink();

            // One of the more flexable ways to access the configuration data is to use the Microsoft's Configuration model,
            // this way we can avoid hard coding the environment secrets. I opted to use the Json and environment variable providers here.

            // Request the instance from the client.
            // Because we're requesting it here first, its targetted constructor will be called and we will receive an active instance.
            var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();

            CommandHandler handler = new(_serviceProvider);
            await handler.InstallCommandsAsync();

            client.Log += async (msg) =>
            {
                await Task.CompletedTask;
                Console.WriteLine(msg);
            };
            //client.Ready += OnReadyAsync;
            await client.LoginAsync(TokenType.Bot, _configuration["token"]);
            await client.StartAsync();

            //Start lavalink

            await Task.Delay(Timeout.Infinite);
        }


    }
}