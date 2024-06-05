using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ai_Chan.Database;

namespace Ai_Chan.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly DatabaseService _database;
        private readonly ConfigurationService _configuration;

        private ulong previousAuthor;

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _database = services.GetRequiredService<DatabaseService>();
            _configuration = services.GetRequiredService<ConfigurationService>();

            _commands.CommandExecuted += CommandExecutedAsync;
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;

            var context = new SocketCommandContext(_discord, message);

            // Respond to greetings
            foreach (string word in new[] { "hello", "hi", "yo", "hey", "ohayo", "henlo", "oi", "ahoy" })
            {
                if (message.Content.Split(" ").Contains(word))
                {
                    await message.AddReactionAsync(Emote.Parse("<:hi:495937399482744842>"));
                }
            }

            // Lottery reaction
            if (new Random().Next(1000) == 1)
            {
                var emote = context.Guild.Emotes.ElementAt(new Random().Next(context.Guild.Emotes.Count));
                await message.AddReactionAsync(emote);
                await message.Channel.SendMessageAsync($"{message.Author.Mention}!! You just won a lottery with 0.001% chance! +10 exp for you for free!");
                _database.AddExp(message.Author.Id, 10);
            }

            // Level up for chatting
            if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "data", "database.db")))
            {
                if (previousAuthor != message.Author.Id && _database.AddExp(message.Author.Id, 1))
                {
                    await message.Channel.SendMessageAsync($"Congratulations {message.Author.Mention}! You leveled up from chatting!");
                }

                previousAuthor = message.Author.Id;
            }

            // Command handling
            var argPos = 0;
            if (!message.HasCharPrefix(char.Parse(_configuration.prefix), ref argPos)) return;

            await _commands.ExecuteAsync(context, argPos, _services);
        }

        private async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified || result.IsSuccess) return;

            await context.Channel.SendMessageAsync($"Error: {result.ErrorReason}");
        }
    }
}
