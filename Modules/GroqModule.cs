using System.Text.Json.Nodes;
using Discord.Commands;
using Discord.WebSocket;
using Ai_Chan.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Ai_Chan.Modules
{
    [Name("AI-Chans braino!")]
    [Summary("**WARNING** overload and matrix errors possible")]
    public class GroqModule : ModuleBase<SocketCommandContext>
    {
        private readonly GroqService _groqService;
        private readonly AgentService _AgentService;

        public GroqModule(GroqService groqService, AgentService agentService)
        {
            _groqService = groqService;
            _AgentService = agentService;
        }

        [Command("ask", RunMode = RunMode.Async)]
        public async Task Ask([Remainder] string text)
        {
            try
            {
                var user = Context.User as SocketGuildUser;
                var displayName = user?.Nickname ?? user?.Username;

                var messages = new List<JsonObject>
                {
                    new JsonObject
                    {
                        ["role"] = "system",
                        ["content"] = _groqService.basicPrompt
                    },
                    new JsonObject
                    {
                        ["role"] = "user",
                        ["content"] = $"{displayName}: {text}"
                    }
                };

                var response = await _groqService.GetResult("llama3-70b-8192", 0.6, 300, messages);

                Console.WriteLine($"Sending response: {response}");
                await Context.Channel.SendMessageAsync(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Ask command: {ex.Message}");
                await Context.Channel.SendMessageAsync("Sorry, something went wrong while processing your request.");
            }
        }

        [Command("chat", RunMode = RunMode.Async)]
        public async Task Chat([Remainder] string text)
        {
            try
            {
                var chatHistory = await _groqService.AssembleChatHistory(Context, text);

                var response = await _groqService.GetResult("llama3-70b-8192", 0.6, 500, chatHistory);

                Console.WriteLine($"Sending response: {response}");
                await Context.Channel.SendMessageAsync(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Chat command: {ex.Message}");
                await Context.Channel.SendMessageAsync("Sorry, something went wrong while processing your request.");
            }
        }
    }
}
