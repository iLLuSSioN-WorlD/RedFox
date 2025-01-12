using Discord;

public interface ICommand
{
    string CommandName { get; }
    Task ExecuteAsync(IMessageChannel channel, IUser user);
}
