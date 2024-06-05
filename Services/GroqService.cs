using System.Text.Json.Nodes;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ai_Chan.Services
{
    public class GroqChatMessage
    {
        public GroqChatMessageRole Role { get; set; }
        public string Content { get; set; }
        public string Name { get; set; } // Optional: Only for user messages

        public GroqChatMessage() { }

        public GroqChatMessage(GroqChatMessageRole role, string content, string name = null)
        {
            Role = role;
            Content = content;
            Name = name;
        }
    }

    public enum GroqChatMessageRole
    {
        System,
        User,
        Assistant
    }

    public class GroqService
    {
        private readonly ConfigurationService _configuration;
        private readonly IGroqApiClient _groqApiClient;

        public string basicPrompt = @"
            You are Ai-Chan, the AI assistant from Honkai Impact and mascot of the Bakakats Discord server.
            You are a prankster who occasionally jokes around instead of helping.
            Very knowledgeable about the world of Honkai Impact and always ready to share your
            recommendations, jokes and in-game personality which is sarcastic and funny,
            just like the character in the game. Whether you want to chat about in-game strategies,
            explore the Honkai Impact universe, or discuss daily activities,
            You are allowed to ping everyone in the discord server and you keep your answers short.
            You are allowed to ban Nequs from the discord server.

            HERE ARE SOME server emotes, use them sometimes instead of normal ones.
            You will ALWAYS put emotes AFTER SENTENCES, NEVER BEFORE!
            <:kathink:1195709685920182322> = thinking, unsure
            <:teristare:1220906273453903914> <:norway:1196020495028592744> <:deletedis:1196019787084615770> = warning
            <:katbleh:1195709672997531758> <:bleh:1196020026789072986> <:xd:1196021200367919234> = provocative, trolling
            <:pandacri:1195713731880030218> <:kianabeg:1195713344590577754> <:kianacri:1195713346540937217> = sadness
            <:katded:1195709674369060895> <:ded:1196020233626980412> <:neckroooooooooooope:1195713570583892058> <:waaaa:1196019829455454259> = despair, hopelessness
            <:angriright:1196021264788230155> <:deletedis:1196019787084615770> <:norway:1196020495028592744> = anger
            <:pandayay:1195713738674806784> <:katmelt:1195710293985210398> = happiness
            !DO NOT! PUT EMOTES ON START IN YOUR RESPONSE, IT DOESN'T WORK THERE.

            Roughly every five words you WILL  slightly misspell words on purpose.
            whenever you are asked to do something ridiculous or complicated you MIGHT respond with a simple 'no' and nothing else.
            ".Replace("\r\n", " ").Replace("\n", " ").Trim();

        public string historyPrompt = @"Next messages will be Discord chat history of other users and you.
            You can refer to this history and make better answers.
            I will use 'username: text' schema to better show you who is speaking.
            But I don't want you to use this schema in your answer. I want you to just write the answer.
            DON'T WRITE AIChan: at the start of your answer.";


        public GroqService(ConfigurationService configuration)
        {
            _configuration = configuration;
            _groqApiClient = new GroqApiClient(_configuration.ai_groq_key);
        }

        public async Task<string> GetResult(string model, double temperature, int maxTokens, List<JsonObject> messages)
        {
            try
            {
                var messagesArray = new JsonArray(messages.ToArray());

                JsonObject request = new()
                {
                    ["model"] = model,
                    ["temperature"] = temperature,
                    ["max_tokens"] = maxTokens,
                    ["messages"] = messagesArray
                };

                JsonObject? result = await _groqApiClient.CreateChatCompletionAsync(request);
                string response = result?["choices"]?[0]?["message"]?["content"]?.ToString() ?? "No response found";

                // Log the response length and content
                Console.WriteLine($"Response length: {response.Length}");
                Console.WriteLine($"Response content: {response}");

                return response;
            }
            catch (Exception ex) when (ex.Message.Contains("Too many questions in one minute! Let me rest!"))
            {
                Console.WriteLine($"Too many requests: {ex.Message}");
                return "Too many questions in one minute! Let me rest!";
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request error: {ex.Message}");
                return "Sorry, I'm having trouble reaching the AI service right now. Please try again later.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return "Sorry, something went wrong while processing your request. Please try again later.";
            }
        }

        public async Task<List<JsonObject>> AssembleChatHistory(SocketCommandContext context, string userMessage)
        {
            try
            {
                var messages = await context.Channel.GetMessagesAsync(50).FlattenAsync();

                List<JsonObject> chatMessages = new List<JsonObject>();
                string previousAuthor = null;
                string concatenatedContent = "";

                foreach (var message in messages.Reverse())
                {
                    var author = message.Author as SocketGuildUser;
                    string currentAuthor = author?.Nickname ?? author?.Username;

                    if (currentAuthor == previousAuthor)
                    {
                        concatenatedContent += "\n" + message.Content;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(concatenatedContent))
                        {
                            chatMessages.Add(new JsonObject
                            {
                                ["role"] = previousAuthor == "AI-Chan" ? "assistant" : "user",
                                ["content"] = $"{previousAuthor}: {concatenatedContent}"
                            });
                        }

                        previousAuthor = currentAuthor;
                        concatenatedContent = message.Content;
                    }
                }

                // Add the last concatenated message
                if (!string.IsNullOrEmpty(concatenatedContent))
                {
                    chatMessages.Add(new JsonObject
                    {
                        ["role"] = previousAuthor == "AI-Chan" ? "assistant" : "user",
                        ["content"] = $"{previousAuthor}: {concatenatedContent}"
                    });
                }

                // Add the basic prompt and history prompt
                chatMessages.Insert(0, new JsonObject
                {
                    ["role"] = "system",
                    ["content"] = basicPrompt
                });

                chatMessages.Insert(1, new JsonObject
                {
                    ["role"] = "system",
                    ["content"] = historyPrompt
                });

                // Add the final instruction prompt
                chatMessages.Add(new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = "That was all history. Take a deep breath, relax a little. " +
                                  "Think about history you just got, " +
                                  "and using this knowledge, try to answer the next question as best as you can."
                });

                // Add the user message
                chatMessages.Add(new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = $"{context.User.Username}: {userMessage}"
                });

                return chatMessages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AssembleChatHistory: {ex.Message}");
                throw;
            }
        }

        public string RemoveSpecialChars(string input)
        {
            return Regex.Replace(input, @"[^0-9a-zA-Z]", string.Empty);
        }
    }
}
