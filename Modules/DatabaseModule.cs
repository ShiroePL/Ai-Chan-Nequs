using Ai_Chan.Services;
using Ai_Chan.Utility;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai_Chan.Modules
{
    [Name("database")]
    [Summary("pls do not mess with it")]
    public class DatabaseModule : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseService _database;
        private readonly DiscordSocketClient _client;

        public DatabaseModule(DatabaseService database, DiscordSocketClient client)
        {
            _database = database;
            _client = client;
        }

        [Command("levelinfo", RunMode = RunMode.Async)]
        public async Task GetUser()
        {
            var info = _database.GetLevelInfo(Context.Message.Author.Id);

            await ReplyAsync($"Gozaimas! o/ How's grinding?\n" +
                             $"Level: {info[0]}\n" +
                            @$"Experience: {info[1]}");
        }

        [Command("aichaninfo", RunMode = RunMode.Async)]
        public async Task AiChanInfo()
        {
            Console.WriteLine(_client.CurrentUser.Id);
            var info = _database.GetLevelInfo(452541322667229194);


            await ReplyAsync($"Here my stats! OwO\n" +
                             $"Level: {info[0]}\n" +
                            @$"Experience: {info[1]}");
        }

        [Command("leaderboard", RunMode = RunMode.Async)]
        public async Task GetLeaderboard()
        {
            await ReplyAsync(_database.GetLeaderboard());
        }

        [Command("addexp", RunMode = RunMode.Async)]
        public async Task AddExp(string number)
        {
            int amount;

            if (int.TryParse(number, out amount))
            {
                _database.AddExp(Context.Message.Author.Id, amount);
                await ReplyAsync("Added " + amount);
            }
        }
    }
}
