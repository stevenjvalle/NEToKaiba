using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace NamazuKingdom // Note: actual namespace depends on the project name.
{
    public class Program
    {
        private readonly IServiceProvider _serviceProvider;

        public Program()
        {
            _serviceProvider = CreateProvider();
        }

        static void Main(string[] args)
            => new Program().RunAsync(args).GetAwaiter().GetResult();

        static IServiceProvider CreateProvider()
        {
            var collection = new ServiceCollection()
                .AddSingleton<DiscordSocketClient>();

            return collection.BuildServiceProvider();
        }

        async Task RunAsync(string[] args)
        {
            // Request the instance from the client.
            // Because we're requesting it here first, its targetted constructor will be called and we will receive an active instance.
            var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();

            client.Log += async (msg) =>
            {
                await Task.CompletedTask;
                Console.WriteLine(msg);
            };

            await client.LoginAsync(TokenType.Bot, "");
            await client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }
    }
}