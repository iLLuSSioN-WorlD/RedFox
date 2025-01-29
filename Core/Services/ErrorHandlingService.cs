using Discord;

namespace DiscordBot.Services
{
    public class ErrorHandlingService : IErrorHandlingService
    {
        public async Task SendErrorMessageAsync(IMessageChannel channel, string message)
        {
            await channel.SendMessageAsync($"❌ **Ошибка:** {message}");
        }
    }
}
