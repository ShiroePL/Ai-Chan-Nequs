using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
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

        private ulong previousAuthor;

        public CommandHandlingService(IServiceProvider services, DiscordSocketClient client, DatabaseService database, GamblingService gambling)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = client;
            _services = services;
            _database = database;
            _gambling = gambling;

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

            if (message.Author.Id == _discord.CurrentUser.Id)
            {
                if (_database.AddExp(message.Author.Id))
                {
                    await message.Channel.SendMessageAsync($"Yaaay! I leveled up! You better start chatting or am gonna be top1!");
                }
            }

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

            if (new Random().Next(10000) == 1)
            {
                var emote = context.Guild.Emotes.ElementAt(new Random().Next(context.Guild.Emotes.Count));
                await message.AddReactionAsync(emote);
                await message.Channel.SendMessageAsync($"Congratulations  {message.Author.Mention}! 0,01% of chances to get that reaction from me! owo");
            }

            if (File.Exists($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                if (previousAuthor != message.Author.Id)
                {
                    if (_database.AddExp(message.Author.Id))
                    {
                        await message.Channel.SendMessageAsync($"Congratulations  {message.Author.Mention}! Your spammerino caused you to level up!\n" +
                                                               $"Make sure to chat everywhere and spam more now its gonna be kinda harder! GRIND GRIND GRIND!");
                    }
                }

                previousAuthor = message.Author.Id;
            }

            if (message.Content == "russian")
            {
                Console.WriteLine("Russian Command Triggered");

                if (!_gambling.Joinable())
                {
                    await _gambling.Russian(message.Author.Id);
                    await message.ReplyAsync($"{message.Author.Mention} has joined the russian!");
                }
                else
                {
                    await message.ReplyAsync("The russian has lready started! You are late! ( ︶︿︶)");
                }

                return;
            }

            if (message.Content == "+slots")
            {
                await _gambling.Slots(message.Author.Id);
                return;
            }

            var prefPos = 0;
            if (!message.HasCharPrefix(char.Parse("+"), ref prefPos)) return;

            await _commands.ExecuteAsync(context, prefPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (context.Message.Author.IsBot)
                return;

            if (!command.IsSpecified)
                return;

            if (result.IsSuccess)
                return;
        }
    }
}