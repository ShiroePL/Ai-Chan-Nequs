using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OpenAI_API.Chat; 
using OpenAI_API.Models;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Ai_Chan.Services
{
    public class DiscordCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public DiscordCommand(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    public class AgentService
    {
        private readonly ConfigurationService _configuration;
        private readonly DiscordSocketClient _client;
        

        public AgentService(ConfigurationService configuration, DiscordSocketClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        public async Task<(string, string)> GetAskResponse(string username, string input)
        {
            try
            {
                var api = new OpenAI_API.OpenAIAPI(_configuration.ai_key);
                string template = PrepareTemplate(input);

                var result = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.GPT4_32k_Context,
                    Temperature = 0.7,
                    MaxTokens = 1000,
                    Messages = new ChatMessage[]
                    {
                        new ChatMessage(ChatMessageRole.System, template)
                    }
                });
                // tu sie zaczyna magia bo mozesz zrobić, 
                // write if result is ping then use ping command from discord
                

                
                string response = result.ToString();
                string pattern = @"Final Answer:\s*(\w+)|Action Input:\s*(\w+)";
                MatchCollection matches = Regex.Matches(response, pattern);

                string extracted_command = "";
                string extracted_action_input = ""; //this is for np ping, to have user nick to ping

                Console.WriteLine($"response is this: {response}");

                foreach (Match match in matches)
                {
                    if (match.Groups[1].Value != "")
                    {
                        extracted_command = match.Groups[1].Value;
                    }
                    if (match.Groups[2].Value != "")
                    {
                        extracted_action_input = match.Groups[2].Value;
                    }
                }

                return (extracted_command, extracted_action_input);
            }
            catch (Exception ex)
            {
                // Log the exception or do something else
                return ("Error", ex.ToString());
            }
        }

        private string PrepareTemplate(string input)
        {
            List<DiscordCommand> discordCommands = new List<DiscordCommand>
            {
                new DiscordCommand("ping", "useful for when discord user wants to ping someone. If information about user is not provided, then bot will ping the user who used the command."),
                new DiscordCommand("slap", "ż"),
                new DiscordCommand("anime_list", "useful for when discord user wants to see his anime list."),
                new DiscordCommand("ban", "useful for when discord user wants to ban someone."),
                new DiscordCommand("leaderboard", "useful for when discord user wants to see his/her leaderboard."),
                new DiscordCommand("help", "useful for when discord user wants to se help information, some instructions or other related stuff."),
                // ... Add other commands here
            };

            string commandNames = string.Join(", ", discordCommands.Select(dc => dc.Name));

            string template = $@"
            Complete the objective as best you can. You're name is Ai-Chan. Your job is to choose which discord command you should use. You have access to the following discord_commands:
            {commandNames}
            Use the following format:
            Question: the input question you must answer
            Thought: you should always think about what to do
            Action: the action to take, should be one of [{commandNames}]
            Action Input: the input to the action/ for ping command it is the user to ping.
            Thought: I now know the final answer
            Final Answer: Name of the command. ONLY NAME OF COMMAND TO USE.
            Begin!
            Question: {input}";

            return template;
        }

        

    }
}
