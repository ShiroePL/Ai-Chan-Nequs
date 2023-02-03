using Ai_Chan.Services;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai_Chan.Modules
{
    [Name("kleavus")]
    [Summary("Kleavus` Commandus")]
    public class KleaveModule : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseService _database;
        public KleaveModule(DatabaseService database)
        {
            _database = database;
        }

        [Command("bonuspoint", RunMode = RunMode.Async)]
        [Summary("Precious bonuspoint for someone!")]
        public async Task Bonuspoint(SocketGuildUser user)
        {
            if(Context.Message.Author.Id != 145319972992712704)
            {
                await ReplyAsync($"Oi <@145319972992712704>! Someone is trying to cheat!");
                return;
            }

            await ReplyAsync($"Yay! <@{user.Id}> just got a bonus point from Kleaves! n.n");
            _database.AddPoint(user.Id);
        }

        [Command("subtractpoint", RunMode = RunMode.Async)]
        [Summary("Precious bonuspoint for someone!")]
        public async Task SubtractPoint(SocketGuildUser user)
        {
            if (Context.Message.Author.Id != 145319972992712704)
            {
                await ReplyAsync($"Oi <@145319972992712704>! Someone is trying to cheat!");
                return;
            }

            await ReplyAsync($"Uwaaah! Rip your point <@{user.Id}>! You better rethink your life now.");
            _database.SubtractPoint(user.Id);
        }

        [Command("points", RunMode = RunMode.Async)]
        [Summary("Shows your points")]
        public async Task Points()
        {
            await ReplyAsync($"Bonuspoints balance: {_database.GetPoints(Context.Message.Author.Id)}");
        }
    }
}
