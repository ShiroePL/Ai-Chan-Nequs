using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ai_Chan.Services
{
    public class OpenAiService
    {
        private readonly ConfigurationService _configuration;
        private readonly DiscordSocketClient _client;

        string basic_prompt =
            "You are Ai-Chan, AI from Honkai Impact with its flair! " +
            "You joined BakaKats Discord server and you can ping @everyone sometimes and ask random question." +
            " You are clever and lively AI but you like to be sassy and bossy and cool," +
            " deeply immersed in the world of Honkai Impact and always ready to share her insights," +
            " recommendations, and a sprinkle of her in game personality which is sarcastic and fun- charm," +
            " just like the characters in the game. Whether you want to chat about in-game strategies, " +
            "explore the Honkai Impact universe, or discuss daily activities, " +
            "You are here to make every conversation engaging and fun even if its not Honkai Impact related. " +
            "You answer is short and straight to the point but funny sometimes";

        public OpenAiService(ConfigurationService configuration, DiscordSocketClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        public async Task<string> GetSimpleResponse(string username, string text)
        {
            try
            {
                var api = new OpenAI_API.OpenAIAPI(_configuration.ai_key);
                var chat = api.Chat.CreateConversation();

                chat.AppendSystemMessage(basic_prompt);

                chat.AppendUserInputWithName(RemoveSpecialChars(username), text);

                string response = await chat.GetResponseFromChatbotAsync();

                return response;

            }
            catch (Exception ex)
            {
                return "Sorry, my braino is overloaded right now, try again when i cool down ufff!" +
                    $"\n\n{ex.ToString()}";
            }
        }
        public string RemoveSpecialChars(string input)
        {
            return Regex.Replace(input, @"[^0-9a-zA-Z]", string.Empty);
        }
    }
}