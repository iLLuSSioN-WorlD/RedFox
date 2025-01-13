using Discord;
using Discord.WebSocket;
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

        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            // Ответ для слэш-команды "ping"
            await command.RespondAsync("Pong!");
        }
    }
}
