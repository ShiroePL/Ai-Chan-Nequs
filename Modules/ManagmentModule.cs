using Discord;
using System.Collections.Generic;
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
using System.Threading;
using System.Linq;
using Ai_Chan.Utility;

namespace Ai_Chan.Modules
{
    [Name("manage")]
    [Summary("Ai-Chan managment commands")]
    public class ManagmentModule : ModuleBase<SocketCommandContext>
    {
        [Command("exile", RunMode = RunMode.Async)]
        [Summary("Kicks someone!")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage = "You don't have required permission to exile people!")]
        [RequireBotPermission(GuildPermission.KickMembers, ErrorMessage = "I don't have required permission to exile people!")]
        public async Task Kick([Remainder]string command)
        {
            SocketGuildUser user = Helpers.ExtractUser(Context, command);

            if (user == null)
            {
                await ReplyAsync("User not found!");
                return;
            }
            else
                await user.KickAsync();

            await ReplyAsync(user.Username + "#" + user.DiscriminatorValue + " has been exiled!");
        }

        [Command("clear", RunMode = RunMode.Async)]
        [Summary("Deletes specified amount of messages")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageMessages, ErrorMessage = "You don't have required permission to delete messages!")]
        [RequireBotPermission(ChannelPermission.ManageMessages, ErrorMessage = "I don't have required permission to delete messages!")]
        public async Task Purge(string i = null)
        {
            IEnumerable<IMessage> messages;

            if (Int32.TryParse(i, out _))
            {
                messages = await Context.Channel.GetMessagesAsync(Int32.Parse(i) + 1).FlattenAsync();

            }
            else
            {
                await Context.Channel.SendMessageAsync("Amount must be a number!\n+clear [amount]");
                return;
            }

            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14));
        }

        [Command("say", RunMode = RunMode.Async)]
        [Summary("Says something as Ai-Chan.")]
        public async Task Say([Remainder]string text)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync(text);
        }

        [Command("showemotes", RunMode = RunMode.Async)]
        [Summary("Lists all of a guild emotes.")]
        [RequireContext(ContextType.Guild)]
        public async Task ShowEmotes()
        {
            List<GuildEmote> animated = new List<GuildEmote>(Context.Guild.Emotes.Where(x => x.Animated));
            List<GuildEmote> standard = new List<GuildEmote>(Context.Guild.Emotes.Where(x => !x.Animated));

            string emotes = "";

            await Context.Channel.SendMessageAsync("Standard emotes | " + standard.Count);

            for (int i = 1; i <= standard.Count; i++)
            {
                if (i % 9 == 0)
                {
                    emotes += "<:" + standard[i - 1].Name + ":" + standard[i - 1].Id + ">";
                    await Context.Channel.SendMessageAsync(emotes);
                    emotes = "";
                }
                else
                {
                    emotes += "<:" + standard[i - 1].Name + ":" + standard[i - 1].Id + ">";
                }
            }

            if (emotes != "")
                await Context.Channel.SendMessageAsync(emotes);

            await Context.Channel.SendMessageAsync("\nAnimated emotes | " + animated.Count);

            emotes = "";

            for (int i = 1; i <= animated.Count; i++)
            {
                if (i % 9 == 0)
                {
                    emotes += "<a:" + animated[i - 1].Name + ":" + animated[i - 1].Id + ">";
                    await Context.Channel.SendMessageAsync(emotes);
                    emotes = "";
                }
                else
                {
                    emotes += "<a:" + animated[i - 1].Name + ":" + animated[i - 1].Id + ">";
                }
            }

            if (emotes != "")
                await Context.Channel.SendMessageAsync(emotes);
        }

        [Command("setnicks", RunMode = RunMode.Async)]
        [Summary("Set new nicknames for users with specific role\nExample: setnicks nickname")]
        public async Task SetNicks(SocketRole userRole = null, [Remainder]string name = null)
        {
            await Context.Message.DeleteAsync();

            if (userRole == null)
            {
                await Context.Channel.SendMessageAsync("Role not found!");
                return;
            }

            foreach (SocketRole role in Context.Guild.Roles)
                if (role.Name == userRole.Name)
                    foreach (SocketGuildUser user in role.Members)
                        await user.ModifyAsync(x => x.Nickname = name == null ? user.Username : name);
        }
    }
}