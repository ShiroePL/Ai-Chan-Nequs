using Ai_Chan.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai_Chan.Modules
{
    [Name("gambling")]
    [Discord.Commands.Summary("Use your exp and try your luck!")]
    public class GamblingModule : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseService _database;
        private readonly DiscordSocketClient _client;

        public GamblingModule(DatabaseService database, DiscordSocketClient client)
        {
            _database = database;
            _client = client;
        }

        [Command("slots", RunMode = Discord.Commands.RunMode.Async)]
        public async Task SlotMachine(string number)
        {
            int amount;

            if (!int.TryParse(number, out amount))
            {
                await Context.Channel.SendMessageAsync($"You need to specify an amount of exp you want to gamble!\n" +
                                                       $"ex. +slots 30");
                return;
            }

            if(amount <= 0)
            {
                await Context.Channel.SendMessageAsync($"Wrong number or you want to spin for free! NOTHINGS FREE HERE! ((╬◣﹏◢))");
                return;
            }

            long exp = _database.GetExp(Context.Message.Author.Id);

            if (exp < (long)amount)
            {
                await Context.Channel.SendMessageAsync($"You don't have enough exp! Better go start grinding ;]");
                return;
            }
            else
            {
                _database.AddExp(Context.Message.Author.Id, -amount);
            }

            // Get the emotes from the server
            //var emotes = Context.Guild.Emotes;
            var emotes = Context.Guild.Emotes.Where(e => e.Animated == false).ToList();

            //Remove booster server emotes, they always last
            emotes = emotes.Take(emotes.Count - 35).ToList();

            // Shuffle the emotes
            var random = new Random();
            var shuffledEmotes = emotes.OrderBy(e => random.Next());

            // Generate the slot machine reels
            var reels = new List<Emote>();
            for (int i = 0; i < 9; i++)
            {
                var randomIndex = random.Next(shuffledEmotes.Count());
                var emote = shuffledEmotes.ElementAt(randomIndex);
                reels.Add(emote);
            }

            // Display the slot machine reels
            string display = $"    {reels[0].ToString()} | {reels[1].ToString()} | {reels[2].ToString()}\n" +
                             $"-- {reels[3].ToString()} | {reels[4].ToString()} | {reels[5].ToString()} -- \n" +
                             $"    {reels[6].ToString()} | {reels[7].ToString()} | {reels[8].ToString()}";

            var selectedReels = reels.Skip(3).Take(3);

            await Context.Channel.SendMessageAsync($"**[ Slot Machine ]**\n{display}");

            // Determine the reward
            if (selectedReels.Distinct().Count() == 1)
            {
                // Jackpot!
                amount *= 10;
                await Context.Channel.SendMessageAsync($"(✯◡✯) **!JACKPOT!** (✯◡✯)\nYou won {amount} Exp!");

                if (_database.AddExp(Context.Message.Author.Id, amount))
                {
                    await Context.Channel.SendMessageAsync($"Congratulations even more {Context.Message.Author.Mention}!\nYour gambling addiction caused you to level up! (* ^ ω ^)");
                }
            }
            else if (selectedReels.Distinct().Count() == 2)
            {
                amount *= 4;
                await Context.Channel.SendMessageAsync($"(￢‿￢ ) **!Two of a Kind!** (￢‿￢ ) \nYou won {amount} Exp!");

                if (_database.AddExp(Context.Message.Author.Id, amount))
                {
                    await Context.Channel.SendMessageAsync($"Congratulations even more {Context.Message.Author.Mention}!\nYour gambling addiction caused you to level up! (* ^ ω ^)");
                }
            }
            else
            {
                // No match
                await Context.Channel.SendMessageAsync($"(⌯˃̶᷄ ﹏ ˂̶᷄⌯) No match! Maybe better spins next time? ");
            }
        }
    }
}
