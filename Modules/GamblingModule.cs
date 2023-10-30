using Ai_Chan.Database;
using Ai_Chan.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Timers;

using static System.Collections.Specialized.BitVector32;

namespace Ai_Chan.Modules
{
    [Name("gambling")]
    [Discord.Commands.Summary("Use your exp and try your luck!")]
    public class GamblingModule : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseService _database;
        private readonly DiscordSocketClient _client;
        private readonly GamblingService _gambling;
        private int time = 0;

        public GamblingModule(DatabaseService database, DiscordSocketClient client, GamblingService gambling)
        {
            _database = database;
            _client = client;
            _gambling = gambling;
        }
        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            time++;
        }

        [Command("russian", RunMode = Discord.Commands.RunMode.Async)]
        public async Task Russian(string number)
        {
            try 
            {
            Console.WriteLine("started");

            if (!_gambling.joinable)
            {
                await Context.Channel.SendMessageAsync("Game has already been started!");
                return;
            }

            int amount;

            if (!int.TryParse(number, out amount))
            {
                await Context.Channel.SendMessageAsync($"You need to specify an amount of exp you want to bet!\n" +
                                                       $"ex. +russian 30");
                return;
            }

            int totalexp = 0;

            // Send a message asking for reactions
            var message = await Context.Channel.SendMessageAsync("React to participate ↑_(ΦwΦ)Ψ");

            // Add a reaction to the message
            await message.AddReactionAsync(new Emoji("\u2620"));

            List<IUser> users = new List<IUser>();

            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimedEvent;
            timer.Start();

            while (time < 15)
            {
                Console.WriteLine("waiting for players to join");
            }

            // Wait for a reaction
            var reactionResult = await message
                .GetReactionUsersAsync(new Emoji("\u2620"), 50)
                .FlattenAsync();

            // If the reaction is null, the wait timed out, so break the loop
            if (!reactionResult.Any())
            {
                await Context.Channel.SendMessageAsync("No more reactions within the time limit.");
                return; 
            }

            Console.WriteLine("after waiting preparing game");


            // Add the users who reacted to the list
            users.AddRange(reactionResult);

            for (int i = users.Count - 1; i >= 0; i--)
            {
                var user = users[i];

                if (user.Id != 452541322667229194)
                {
                    if (_database.GetExp(user.Id) < amount)
                    {
                        await Context.Channel.SendMessageAsync($"{user.Mention}!!!! is broke as heck and cannot join\n" +
                            $"Go get some exp and dont waste OUR time");
                        users.RemoveAt(i); // remove element at index i
                    }
                    else
                    {
                        _database.AddExp(user.Id, -amount);
                        totalexp += amount;
                    }
                }
            }

            // Send a message containing the list of users who reacted
            if (users.Count > 2)
            {
                var text = "ATTENTION CATTOS!  " +
                    string.Join(", ", users.Select(u => $"<@{u.Id}>"));

                await Context.Channel.SendMessageAsync(text + "\nTHE GAME IS ABOUT TO BEGIN!");

                await _gambling.RussianGame(users, Context, totalexp);
            }
            else
            {
                await Context.Channel.SendMessageAsync("EVERYONE scared to join huh? Minimum 2 cattos required for the game to start!");
            }

            time = 0;
            totalexp = 0;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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
            emotes = emotes.Take(emotes.Count - 40).ToList();

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
        // rps 101 
        public class WinningLossing
        {
            [JsonPropertyName("winner")]
            public string winner { get; set; }

            [JsonPropertyName("outcome")]
            public string outcome { get; set; }

            [JsonPropertyName("loser")]
            public string loser { get; set; }
        }

        [Command("rps")]
        public async Task RPS101(string thing, int bet)
        {
            try
            {
                // Handling points
                if (bet <= 0)
                {
                    await Context.Channel.SendMessageAsync("Invalid bet amount. Please specify a positive bet.");
                    return;
                }

                long exp = _database.GetExp(Context.User.Id);

                if (exp < bet)
                {
                    await Context.Channel.SendMessageAsync("You don't have enough exp to place that bet.");
                    return;
                }
                else
                {
                    _database.AddExp(Context.User.Id, -bet);
                }

                // Fetch items and Aichan's choice
                HttpClient client = new HttpClient();
                string itemsResponse = await client.GetStringAsync("https://rps101.pythonanywhere.com/api/v1/objects/all");
                var items = JsonSerializer.Deserialize<List<string>>(itemsResponse);//

                Random random = new Random();
                int AichanChoiceIndex = random.Next(items.Count - 1);
                string AichanChoice = items[AichanChoiceIndex];

                // Fetch match result
                string matchResponse = await client.GetStringAsync("https://rps101.pythonanywhere.com/api/v1/match?object_one=" + thing + "&object_two=" + AichanChoice);
                var winningLossing = JsonSerializer.Deserialize<WinningLossing>(matchResponse);


                Console.WriteLine(winningLossing.winner);
                Console.WriteLine(winningLossing.outcome);
                Console.WriteLine(winningLossing.loser);
                Console.WriteLine(thing);
                Console.WriteLine(matchResponse);

                if (winningLossing == null)
                {
                    
                }

                // Check who won, lost, or if it's a draw
                if (winningLossing.winner.ToLower() == thing.ToLower())
                {
                    await Context.Channel.SendMessageAsync("You won! " + winningLossing.winner + " " + winningLossing.outcome + " " + winningLossing.loser);

                    if (_database.AddExp(Context.User.Id, bet))
                    {
                        await Context.Channel.SendMessageAsync($"Congratulations {Context.User.Mention}!\nYour undoubted win caused you to level up ヽ(^o^)ノ");
                    }
                }
                else if (winningLossing.loser.ToLower() == thing.ToLower())
                {
                    await Context.Channel.SendMessageAsync("You lost! " + winningLossing.winner + " " + winningLossing.outcome + " " + winningLossing.loser + ". Better luck next time!");
                }
                else
                {
                    await Context.Channel.SendMessageAsync("It's a draw. Better luck next time ;)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
/*  make embed to display commands
 *  make answers generated by ai
 */
