using Discord;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class PingCommand : ICommand
    {
        public string CommandName => "ping";

        public async Task ExecuteAsync(IMessageChannel channel, IUser user)
        {
            // Ответ на команду "ping"
            await channel.SendMessageAsync($"Pong! Привет, <@{user.Id}>!");
        }
    }
}