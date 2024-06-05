using Discord.Commands;

namespace Ai_Chan.Modules
{
    public interface IOpenAiModule
    {
        Task Oldask([Remainder] string text);
        Task Oldchat([Remainder] string text);
        Task Vision([Remainder] string text);
    }
}