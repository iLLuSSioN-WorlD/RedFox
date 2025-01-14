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
            await channel.SendMessageAsync($"Pong! Привет, <@{user.Id}>!");
        }

        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("Pong!");
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
