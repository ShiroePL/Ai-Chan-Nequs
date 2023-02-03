using Discord;
using Discord.Webhook;
using Discord.WebSocket;
using Discord.Commands;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Ai_Chan.Services;
using Ai_Chan.Utility;

namespace Ai_Chan.Modules
{
    [Name("info")]
    [Summary("Contains all needed commands, get information about servers, users and Ai-Chan.")]
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly DiscordSocketClient _client;

        public InfoModule(CommandService service, DiscordSocketClient client)
        {
            _service = service;
            _client = client;
        }

        [Command("help", RunMode = RunMode.Async)]
        public async Task Help(string arg = null)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            embedBuilder.Color = Color.Purple;
            embedBuilder.WithAuthor(author =>
            {
                author.WithName(_client.CurrentUser.Username);
                author.WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl());
            });

            if (arg != null)
            {
                foreach (var module in _service.Modules)
                {
                    if (arg == module.Name)
                    {
                        foreach (var command in module.Commands)
                            if (command.Name != "help")
                                embedBuilder.AddField("+" + (module.Group == null ? string.Empty : (module.Group + " ")) + command.Name, command.Summary == null ? "No Summary" : command.Summary, false);

                        await Context.Channel.SendMessageAsync(embed: embedBuilder.Build());
                        return;
                    }
                }
            }

            //if module not found, list modules avaiable
            foreach (var module in _service.Modules)
                embedBuilder.AddField(char.ToUpper(module.Name[0]) + module.Name.Substring(1) + " Module | " + "+help " + module.Name, module.Summary, false);

            await Context.Channel.SendMessageAsync(embed: embedBuilder.Build());
        }

        [Command("latency", RunMode = RunMode.Async)]
        [Summary("Shows Ai-Chan's response time")]
        public async Task Latency() => await ReplyAsync("My response time is " + Context.Client.Latency + " ms.");

        [Command("botinfo", RunMode = RunMode.Async)]
        [Summary("Shows Ai-Chans' statistics")]
        public async Task Stats()
        {
            int users = 0;
            foreach (SocketGuild guild in Context.Client.Guilds)
                users += guild.Users.Count;

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.Color = Color.Purple;

            embedBuilder.WithAuthor(author =>
            {
                author.WithName("Ai-Chan");
                author.WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl());
            });

            embedBuilder.AddField("Guilds", Context.Client.Guilds.Count, true);
            embedBuilder.AddField("Users", users, true);
            embedBuilder.AddField("Prefix", "+", true);
            embedBuilder.AddField("Created", Context.Client.CurrentUser.CreatedAt.UtcDateTime, true);
            embedBuilder.AddField("ID", Context.Client.CurrentUser.Id, true);

            await Context.Channel.SendMessageAsync(embed: embedBuilder.Build());
        }

        [Command("userinfo", RunMode = RunMode.Async)]
        [Summary("Shows user's information \n+userinfo [user]")]
        public async Task Info([Remainder] string command)
        {
            SocketGuildUser user = Helpers.ExtractUser(Context, command);

            string roles = "| ";
            foreach (IRole role in user.Roles)
                if (role.Name != "@everyone")
                    roles += role.Mention + " | ";

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.Color = Color.Purple;

            embedBuilder.WithAuthor(author =>
            {
                author.WithName(user.Username + "#" + user.Discriminator);
                author.WithIconUrl(user.GetAvatarUrl());
            });

            embedBuilder.AddField("Joined", user.JoinedAt.Value.UtcDateTime, true);
            embedBuilder.AddField("Registered", user.CreatedAt.UtcDateTime, true);
            embedBuilder.AddField("Status", user.Status, false);
            embedBuilder.AddField("Roles", roles, false);
            embedBuilder.AddField("Avatar", user.GetAvatarUrl(), false);

            await Context.Channel.SendMessageAsync(embed: embedBuilder.Build());
        }

        [Command("serverinfo", RunMode = RunMode.Async)]
        [Summary("Shows information about current guild the command is executed in")]
        public async Task ServerInfo()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.Color = Color.Purple;

            embedBuilder.WithAuthor(author =>
            {
                author.WithName(Context.Guild.Name);
                author.WithIconUrl(Context.Guild.IconUrl);
            });

            embedBuilder.AddField("Created", Context.Guild.CreatedAt.UtcDateTime, true);
            embedBuilder.AddField("Channel categories", Context.Guild.CategoryChannels.Count, true);
            embedBuilder.AddField("Text channels", Context.Guild.TextChannels.Count, true);
            embedBuilder.AddField("Voice channels", Context.Guild.VoiceChannels.Count, true);
            embedBuilder.AddField("Emotes", Context.Guild.Emotes.Count, true);
            embedBuilder.AddField("ID", Context.Guild.Id, true);
            embedBuilder.AddField("Members", Context.Guild.MemberCount, true);
            embedBuilder.AddField("Owner", Context.Guild.Owner.Username + "#" + Context.Guild.Owner.DiscriminatorValue, true);
            embedBuilder.AddField("Owner's ID", Context.Guild.Owner.Id, true);
            embedBuilder.AddField("Roles", Context.Guild.Roles.Count, true);
            embedBuilder.AddField("Boost tier", Context.Guild.PremiumTier, true);
            embedBuilder.AddField("Boosts", Context.Guild.PremiumSubscriptionCount, true);
            embedBuilder.AddField("Icon", Context.Guild.IconUrl, true);

            await Context.Channel.SendMessageAsync(embed: embedBuilder.Build());
        }

        [Command("avatar", RunMode = RunMode.Async)]
        [Summary("Sends link to user's avatar")]
        public async Task Avatar([Remainder] string command)
        {
            SocketGuildUser user = Helpers.ExtractUser(Context, command);
            await Context.Channel.SendMessageAsync(user.GetAvatarUrl(size: 1024));
        }
    }
}