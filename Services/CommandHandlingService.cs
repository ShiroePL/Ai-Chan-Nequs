using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Ai_Chan.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly DatabaseService _database;
        private readonly GamblingService _gambling;
        private readonly ConfigurationService _configuration;

        private ulong previousAuthor;

        public CommandHandlingService(IServiceProvider services, DiscordSocketClient client, DatabaseService database, ConfigurationService configuration)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = client;
            _services = services;
            _database = database;
            _configuration = configuration;

            _commands.CommandExecuted += CommandExecutedAsync;
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;

            if (message.Source != MessageSource.User) return;

            var context = new SocketCommandContext(_discord, message);

            foreach (string word in new string[] { "hello", "hi", "yo", "hey", "ohayo", "henlo", "oi", "ahoy" })
            {
                foreach(string s in message.Content.Split(" "))
                {
                    if(word == s)
                    {
                        await message.AddReactionAsync(Emote.Parse("<:hi:495937399482744842>"));
                    }
                }
            }

            if (new Random().Next(1000) == 1)
            {
                var emote = context.Guild.Emotes.ElementAt(new Random().Next(context.Guild.Emotes.Count));
                await message.AddReactionAsync(emote);
                await message.Channel.SendMessageAsync($"{message.Author.Mention}!! You just won a lottery with 0.001% to win! +10 exp for you for free!");

                _database.AddExp(message.Author.Id, 10);
            }

            string dataDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "data");
            string path = Path.Combine(dataDirectory, "database.db");

            if (File.Exists(path))
            {
                if (previousAuthor != message.Author.Id)
                {
                    if (_database.AddExp(message.Author.Id, 1))
                    {
                        await message.Channel.SendMessageAsync($"Congratulations  {message.Author.Mention}! Your spammerino caused you to level up!\n" +
                                                               $"Make sure to chat everywhere and spam more now its gonna be kinda harder! GRIND GRIND GRIND!");
                    }
                }

                previousAuthor = message.Author.Id;
            }

            var prefPos = 0;
            if (!message.HasCharPrefix(char.Parse(_configuration.prefix), ref prefPos)) return;

            await _commands.ExecuteAsync(context, prefPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (context.Message.Author.IsBot)
                return;

            if (!command.IsSpecified)
                return;

            if (result.IsSuccess)
            {
                if (_database.AddExp(_discord.CurrentUser.Id, 1))
                {
                    await context.Channel.SendMessageAsync($"Yaaay! I leveled up! You better start chatting or am gonna be top1!");
                }
            }
        }
    }
}