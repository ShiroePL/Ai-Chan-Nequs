using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ai_Chan.Services
{
    public class GroqApiClient : IGroqApiClient
    {
        private readonly HttpClient client = new();

        public GroqApiClient(string apiKey)
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public async Task<JsonObject?> CreateChatCompletionAsync(JsonObject request)
        {
            try
            {
                var jsonString = request.ToString();
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Print the request content
                Console.WriteLine("Request Content: " + jsonString);

                var response = await client.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    throw new HttpRequestException("Too many requests: 429");
                }

                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonNode.Parse(responseBody)?.AsObject();
            }
            catch (HttpRequestException httpEx) when (httpEx.Message.Contains("429"))
            {
                Console.WriteLine($"HTTP Request error: {httpEx.Message}");
                throw new Exception("Too many questions in one minute! Let me rest!", httpEx);
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request error: {httpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }

        public async IAsyncEnumerable<JsonObject?> CreateChatCompletionStreamAsync(JsonObject request)
        {
            JsonObject? responseJson = null;
            try
            {
                var content = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    Console.WriteLine("HTTP Request error: Too many requests: 429");
                    yield break;
                }

                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                responseJson = JsonNode.Parse(responseBody)?.AsObject();
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            if (responseJson != null)
            {
                yield return responseJson;
            }
            else
            {
                yield break;
            }
        }
    }
}
