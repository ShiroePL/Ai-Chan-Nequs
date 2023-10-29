using Ai_Chan.Services;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;


namespace Ai_Chan.Modules
{
    [Name("api")]
    [Summary("Services offered by Ai-Chan - paid 1 exp per use")]
    public class ApiModule : ModuleBase<SocketCommandContext>
    {
        private DatabaseService _database;
        private DiscordSocketClient _client;

        public ApiModule(DatabaseService database, DiscordSocketClient client)
        {
            _database = database;
            _client = client;
        }

        public class List
        {
            [JsonPropertyName("definition")]
            public string definition { get; set; }

            [JsonPropertyName("permalink")]
            public string permalink { get; set; }

            [JsonPropertyName("thumbs_up")]
            public int thumbs_up { get; set; }

            [JsonPropertyName("author")]
            public string author { get; set; }

            [JsonPropertyName("word")]
            public string word { get; set; }

            [JsonPropertyName("defid")]
            public int defid { get; set; }

            [JsonPropertyName("current_vote")]
            public string current_vote { get; set; }

            [JsonPropertyName("written_on")]
            public DateTime written_on { get; set; }

            [JsonPropertyName("example")]
            public string example { get; set; }

            [JsonPropertyName("thumbs_down")]
            public int thumbs_down { get; set; }
        }

        public class Root
        {
            [JsonPropertyName("list")]
            public List<List> list { get; set; }
        }

        [Command("urban")]
        [Summary("urban dictionary")]
        public async Task GetUrbanDictionaryDefinition([Remainder] string term)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await new HttpClient().GetStringAsync($"https://api.urbandictionary.com/v0/define?term={term}");
                    Root urbanResponse = JsonSerializer.Deserialize<Root>(response);

                    int pageSize = 2; // Number of definitions to display per page
                    var currentPage = 1;

                    var results = urbanResponse.list.ToList().Take(pageSize).ToList();

                    if(results.Count == 0)
                    {
                        await ReplyAsync("Nothing's here");
                        return;
                    }

                    var message = await ReplyAsync(embed: GetDefinitionEmbed(term, results));
                    var customEmoji = new Emoji("<:kiana:1093963437522038936>");
                    await message.AddReactionAsync(customEmoji); 

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private Embed GetDefinitionEmbed(string term, List<List> results)
        {
            var builder = new EmbedBuilder()
                .WithTitle($"Most voted Urban Dictionary definitions for \"{term}\"")
                .WithColor(Color.Blue);

            foreach (var result in results)
            {
                builder.AddField(result.word, $"{result.definition}\n\n*Example:* {result.example}\n*Author:* {result.author}\n*Link:* {result.permalink}");
            }

            return builder.Build();
        }


    }
}
