﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using Ai_Chan.Services;
using System.Reflection;
using Discord.Interactions;

namespace Ai_Chan
{
    class Program
    {
        static void Main() => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                var database = services.GetRequiredService<DatabaseService>();
                var events = services.GetRequiredService<EventsService>();
                var interaction = services.GetRequiredService<InteractionService>();

                await CheckDatabase(client, database);

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                await client.LoginAsync(TokenType.Bot, @"NDUyNTQxMzIyNjY3MjI5MTk0.G-QXbX.AvLNSfjWFuJNd1c3Q5VTO8ff-Bz8tAErypylfw");
                await client.StartAsync();

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                await client.SetGameAsync("Honkai Impact 3rd");

                await Task.Delay(-1);
            }
        }

        private Task CheckDatabase(DiscordSocketClient _client, DatabaseService _database)
        {
            if (!File.Exists($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                foreach (var guild in _client.Guilds)
                {
                    foreach (var user in guild.Users)
                    {
                        _database.AddUser(user);
                    }
                }

                Console.WriteLine("Database has been created!");

                return Task.CompletedTask;
            }

            Console.WriteLine("Database already exists!");
            return Task.CompletedTask;
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine($"{DateTime.Now} | {log.Message}");
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true,
                    GatewayIntents = GatewayIntents.All,
                    LogLevel = LogSeverity.Debug
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    LogLevel = LogSeverity.Debug
                }))
                .AddSingleton<DatabaseService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<EventsService>()
                .AddSingleton<GamblingService>()
                .AddSingleton<InteractionService>()
                .BuildServiceProvider();
        }
    }
}