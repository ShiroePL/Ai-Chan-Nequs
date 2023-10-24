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
        private readonly AgentService _AgentService;
        public OpenAiModule(OpenAiService openAiService, AgentService agentService)
        {
            _openAiService = openAiService;
            _AgentService = agentService;
        }
        
        [Command("chat", RunMode = RunMode.Async)]
        public async Task Ask([Remainder] string text)
        {
            string response = await _openAiService.GetChatResponse(Context, Context.Message.Author.Username, text);
            await Context.Channel.SendMessageAsync(response);
        }

        [Command("ask", RunMode = RunMode.Async)]
        public async Task Chat([Remainder] string text)
        {
            string response = await _openAiService.GetAskResponse(Context.Message.Author.Username, text);
            await Context.Channel.SendMessageAsync(response);
        }


        [Command("agent", RunMode = RunMode.Async)]
        public async Task AgentCommand([Remainder] string text)
        {
            (string extractedCommand, string extractedActionInput) = await _AgentService.GetAskResponse(Context.Message.Author.Username, text);
            await Context.Channel.SendMessageAsync(extractedCommand + " " + extractedActionInput);
        }


        // this needs to be cahnged to new user name system
        // [Command("agent", RunMode = RunMode.Async)]
        // public async Task AgentCommand([Remainder] string text)
        // {
        //     (string extractedCommand, string extractedActionInput) = await _AgentService.GetAskResponse(Context.Message.Author.Username, text);
            
        //     if(extractedCommand == "ping")
        //     {
        //         // Search by DisplayName
        //         var user = Context.Guild.Users.FirstOrDefault(u => u.DisplayName == extractedActionInput);
                
        //         if(user != null)
        //         {
        //             await Context.Channel.SendMessageAsync($"{user.Mention}, you've been pinged!"); // This will mention the user
        //         }
        //         else
        //         {
        //             await Context.Channel.SendMessageAsync("User not found.");
        //         }
        //     }
        //     else
        //     {
        //         await Context.Channel.SendMessageAsync(extractedCommand); // or whatever you want to do with the extracted command
        //     }
        // }
        
    }
}
