using Discord.Commands;

namespace Ai_Chan.Modules
{
    public interface IOpenAiModule
    {
        Task Ask([Remainder] string text);
        Task Chat([Remainder] string text);
        Task Vision([Remainder] string text);
    }
}