using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    public class PingCommand : ICommand
    {
        public string CommandName => "ping";

        // Для текстовых команд
        public async Task ExecuteAsync(IMessageChannel channel, IUser user, string[] args)
        {
            await channel.SendMessageAsync($"Pong! Привет, <@{user.Id}>!");
        }

        // Для слэш-команд
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("Pong!");
        }

        // Для кнопок
        public async Task ExecuteComponentCommandAsync(SocketMessageComponent component) // ✅ Реализован метод для кнопок
        {
            await component.RespondAsync("Pong!");
        }

        public ApplicationCommandProperties RegisterSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("ping")
                .WithDescription("Ответить Pong!")
                .Build();
        }
    }
}
