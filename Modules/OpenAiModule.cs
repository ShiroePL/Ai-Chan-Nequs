using Discord.Commands;
using Ai_Chan.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Ai_Chan.Database;
using System.Text.RegularExpressions;

namespace Ai_Chan.Modules
{
    [Name("AI-Chans braino!")]
    [Summary("**WARNING** overload and matrix errors possible")]
    public class OpenAiModule : ModuleBase<SocketCommandContext>
    {
        private readonly OpenAiService _openAiService;

        public OpenAiModule(OpenAiService openAiService)
        {
            _openAiService = openAiService;
        }

        [Command("aichan", RunMode = RunMode.Async)]
        public async Task Ask([Remainder] string text)
        {
            string response = await _openAiService.GetSimpleResponse(Context.Message.Author.Username, text);
            await Context.Channel.SendMessageAsync(response);
        }
    }
}
