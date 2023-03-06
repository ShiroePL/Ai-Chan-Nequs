using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ai_Chan.Utility
{
    public static class Helpers
    {
        public static SocketGuildUser ExtractUser(SocketCommandContext ctx, string message)
        {
            string impliedUser = message.TrimStart().TrimEnd();

            if (impliedUser.StartsWith("<@"))
            {
                return ctx.Guild.GetUser(ctx.Message.MentionedUsers.First().Id);
            }
            else
            {
                try
                {
                    return ctx.Guild.GetUser(UInt64.Parse(impliedUser));
                }
                catch
                {
                    foreach (var user in ctx.Guild.Users)
                    {
                        if (user.Nickname == impliedUser || user.Username == impliedUser)
                        {
                            return user;
                        }
                    }
                }
            }
            return null;
        }

        public static async Task<string> GetHttpResponseString(string url)
        {
            HttpClient client = new HttpClient();
            return await client.GetStringAsync(url);
        }

        public static async Task<string> GetResposeFromAi(string url)
        {
            HttpClient client = new HttpClient();
            return await client.GetStringAsync(url);
        }

    }
}