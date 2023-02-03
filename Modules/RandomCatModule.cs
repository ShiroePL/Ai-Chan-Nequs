using Ai_Chan.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai_Chan.Modules
{
    [Name("randomcat")]
    [Summary("Random kitties or nekos!")]
    public class RandomCatModule : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseService _database;
        public RandomCatModule(DatabaseService database)
        {
            _database = database;
        }

        [Command("addkitty", RunMode = RunMode.Async)]
        [Summary("Add a link to randomkitty database!\nExample: addkitty https://i.imgur.com/lQcGEtY.png")]
        public async Task AddKitty([Remainder]string link)
        {
            await Context.Message.DeleteAsync();
            _database.AddPicture("kitty", link);
            await Context.Channel.SendMessageAsync($"{Context.Message.Author.Mention} has added a new kitty!\n{link}");
        }

        [Command("randomkitty", RunMode = RunMode.Async)]
        [Summary("Random kitty is back!")]

        public async Task RandomKitty()
        {
            await ReplyAsync(_database.GetRandomPicture("kitty"));
        }

        [Command("addneko", RunMode = RunMode.Async)]
        [Summary("Add a link to anime neko database!\nExample: addneko https://i.imgur.com/KxBmblj.jpg")]
        public async Task AddNeko([Remainder]string link)
        {
            await Context.Message.DeleteAsync();
            _database.AddPicture("neko", link);
            await Context.Channel.SendMessageAsync($"{Context.Message.Author.Mention} has added a new neko!\n{link}");
        }

        [Command("randomneko", RunMode = RunMode.Async)]
        [Summary("Random anime neko is also back!")]
        public async Task RandomNeko()
        {
            await ReplyAsync(_database.GetRandomPicture("neko"));
        }

        [Command("shownekos", RunMode = RunMode.Async)]
        [Summary("Shows all nekos")]

        public async Task ShowNekos()
        {
            var links = _database.GetPictures("neko");
            for (int i = 0; i < links.Count; i++)
            {
                await Context.Channel.SendMessageAsync($"{i}. {links[i]}");
            }
        }

        [Command("showkitties", RunMode = RunMode.Async)]
        [Summary("Shows all kitties")]
        public async Task ShowKitties()
        {
            var links = _database.GetPictures("kitty");
            for (int i = 0; i < links.Count; i++)
            {
                await Context.Channel.SendMessageAsync($"{i}. {links[i]}");
            }
        }
    }
}
