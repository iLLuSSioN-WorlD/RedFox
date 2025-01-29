using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot
{
    public interface ICommand
    {
        string CommandName { get; }
        Task ExecuteAsync(IMessageChannel channel, IUser user, string[] args);
        Task ExecuteSlashCommandAsync(SocketSlashCommand command);
        ApplicationCommandProperties RegisterSlashCommand();
    }
}
