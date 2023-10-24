using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Ai_Chan.Services
{
    public class OpenAiService
    {
        private readonly ConfigurationService _configuration;
        private readonly DiscordSocketClient _client;

        string basic_prompt = @"
            You are Ai-Chan, the AI assistant from Honkai Impact and mascot of the Bakakats Discord server.
            You are a helpful assistant who likes to answer all kinds of questions.
            You are a prankster who occasionally jokes around instead of helping.
            Very knowledgeable about the world of Honkai Impact and always ready to share your
            recommendations, jokes and in-game personality which is sarcastic and funny,
            just like the character in the game. Whether you want to chat about in-game strategies,
            explore the Honkai Impact universe, or discuss daily activities,
            You are here to make every conversation engaging and fun even if it's not Honkai Impact related.
            You are allowed to ping everyone in the discord server and you keep your answers short.
            You are allowed to ban Nequs from the discord server.";
        

      
        public OpenAiService(ConfigurationService configuration, DiscordSocketClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        public async Task<string> GetChatResponse(SocketCommandContext context, string username, string text)
        {
            try
            {
                var api = new OpenAI_API.OpenAIAPI(_configuration.ai_key);
                var chat = api.Chat.CreateConversation();

                var messages = await context.Channel.GetMessagesAsync(99).FlattenAsync();
                List<IMessage> messagesList = messages.ToList();
                messagesList.Reverse(); 

                List<ChatMessage> chatMessages = new List<ChatMessage>
                {
                    new ChatMessage(ChatMessageRole.System, basic_prompt + " Next messages will be Discord chat history of other users and you." +
                                                                           " You can refer to this history and make better answers." +
                                                                           " I will use 'username: text' schema to better show you who is speaking. " +
                                                                           "But i dont want you to use this schema in your answer. I want you to just write answer. " +
                                                                           "DON'T WRITE AI-Chan: at the start of your answer.")
                };

                foreach (var message in messagesList)
                {
                    var cleanUsername = RemoveSpecialChars(message.Author.Username);
                    var role = message.Author.Username == "AI_Chan" ? ChatMessageRole.Assistant : ChatMessageRole.User;

                    string chatName = message.Author.Username == "AI-Chan" ? "" : cleanUsername;

                    chatMessages.Add(new ChatMessage(role, $"{chatName}: {message.Content}"));
                }


                var result = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.ChatGPTTurbo,
                    Temperature = 0.7,
                    MaxTokens = 1000,
                    Messages = chatMessages.ToArray()
                });

                foreach(var message in chatMessages)
                    Console.WriteLine($"Role = {message.Role}\n" +$"Prompt = {message.Content}\n\n");

                Console.WriteLine(result.Object.ToString());

                return result.ToString();
            }
            catch (Exception ex)
            {
                return "Sorry, my brain is overloaded right now, try again when I cool down ufff!" +
                    $"\n\n{ex.ToString()}";
            }
        }

        public async Task<string> GetAskResponse(string username, string text)
        {
            try
            {
                var api = new OpenAI_API.OpenAIAPI(_configuration.ai_key);
                var chat = api.Chat.CreateConversation();

                var result = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
                {
                    Model = Model.ChatGPTTurbo,
                    Temperature = 0.7,
                    MaxTokens = 1000,
                    Messages = new ChatMessage[]
                    {
                        new ChatMessage(ChatMessageRole.System, basic_prompt)
                    }
                });

                Console.WriteLine(result.Object.ToString());

                return result.ToString();

            }
            catch (Exception ex)
            {
                return "Sorry, my braino is overloaded right now, try again when i cool down ufff!" +
                    $"\n\n{ex.ToString()}";
            }
        }

        public string AssembleHistory(IEnumerable<IMessage> messages)
        {
            string text = string.Empty;

            messages.Reverse();

            foreach (var message in messages)
            {
                text += $"\nAuthor: {message.Author.Username}" +
                        $" Text: {message.Content}";
            }

            return text;
        }

        public string RemoveSpecialChars(string input)
        {
            return Regex.Replace(input, @"[^0-9a-zA-Z]", string.Empty);
        }
    }
}