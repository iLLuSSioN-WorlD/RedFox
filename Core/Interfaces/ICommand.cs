using Discord.WebSocket;
using Discord;

public interface ICommand
{
    string CommandName { get; }
    Task ExecuteAsync(IMessageChannel channel, IUser user, string[] args);
    Task ExecuteSlashCommandAsync(SocketSlashCommand command);
    ApplicationCommandProperties RegisterSlashCommand();
}
