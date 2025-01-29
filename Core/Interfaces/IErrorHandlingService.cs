using Discord;

namespace DiscordBot.Services
{
    public interface IErrorHandlingService
    {
        Task SendErrorMessageAsync(IMessageChannel channel, string message);
    }
}
