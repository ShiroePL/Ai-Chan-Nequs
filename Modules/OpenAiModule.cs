﻿using Discord.Commands;
using Ai_Chan.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Ai_Chan.Database;
using System.Text.RegularExpressions;
using OpenAI_API.Models;
using OpenAI_API.Chat;

namespace Ai_Chan.Modules
{
    [Name("AI-Chans braino!")]
    [Summary("**WARNING** overload and matrix errors possible")]
    public class OpenAiModule : ModuleBase<SocketCommandContext>, IOpenAiModule
    {
        private readonly OpenAiService _openAiService;
        private readonly AgentService _AgentService;

        public OpenAiModule(OpenAiService openAiService, AgentService agentService)
        {
            _openAiService = openAiService;
            _AgentService = agentService;
        }

        [Command("oldchat", RunMode = RunMode.Async)]
        public async Task Oldchat([Remainder] string text)
        {
            ChatMessage[] chatHistory = await _openAiService.AssembleChatHistory(Context, text);

            string response = await _openAiService.GetResult(Model.GPT4_Turbo, 0.8, 500, chatHistory);
            await Context.Channel.SendMessageAsync(response);
        }

        [Command("oldask", RunMode = RunMode.Async)]
        public async Task Oldask([Remainder] string text)
        {
            string response = await _openAiService.GetResult(Model.GPT4_Turbo, 0.1, 1000, new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.System, _openAiService.basicPrompt),
                new ChatMessage(ChatMessageRole.User, text)
            });

            await Context.Channel.SendMessageAsync(response, isTTS: false);
        }
        
        [Command("tts", RunMode = RunMode.Async)]
        public async Task AskTTS([Remainder] string text)
        {
            string response = await _openAiService.GetResult(Model.GPT4_Turbo, 0.1, 1000, new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.System, _openAiService.basicPrompt),
                new ChatMessage(ChatMessageRole.User, text)
            });
        
            // Use the SendMessageAsync method with TTS enabled
            await Context.Channel.SendMessageAsync(response, isTTS: true);
        }

        
        
        [Command("vision", RunMode = RunMode.Async)]
        public async Task Vision([Remainder] string text)
        {
            string response = "";

            if (Context.Message.Attachments.Count != 0)
            {
                var attachements = Context.Message.Attachments.Where(x =>
                x.Filename.EndsWith(".jpg") || x.Filename.EndsWith(".png")); // or what you want as "image"

                if (attachements.Any())
                {
                    response = await _openAiService.GetResultVision(attachements.FirstOrDefault().Url, text);
                }
                else
                {
                    response = "nothing there!";
                }


                await Context.Channel.SendMessageAsync(response);
            }
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