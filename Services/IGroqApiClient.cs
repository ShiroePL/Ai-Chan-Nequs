using System.Text.Json.Nodes;

namespace Ai_Chan.Services
{
    public interface IGroqApiClient
    {
        Task<JsonObject?> CreateChatCompletionAsync(JsonObject request);
        IAsyncEnumerable<JsonObject?> CreateChatCompletionStreamAsync(JsonObject request);
    }
}
