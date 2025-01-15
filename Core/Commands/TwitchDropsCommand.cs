using Discord.WebSocket;
using Discord;
using DiscordBot;

public class TwitchDropsCommand : ICommand
{
    private readonly TwitchDropsScraper _scraper;

    public TwitchDropsCommand(TwitchDropsScraper scraper)
    {
        _scraper = scraper;
    }

    public string CommandName => "twitchdrops";

    public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
    {
        try
        {
            // Уведомляем Discord, что команда обрабатывается
            await command.DeferAsync();

            // Выполняем парсинг Twitch Drops
            var drops = await _scraper.GetTwitchDropsAsync();

            // Отправляем результат
            await command.FollowupAsync(string.IsNullOrWhiteSpace(drops)
                ? "Сейчас нет активных Twitch Drops."
                : $"Актуальные Twitch Drops:\n{drops}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке команды: {ex.Message}");
            await command.FollowupAsync("Произошла ошибка при выполнении команды.", ephemeral: true);
        }
    }

    public async Task ExecuteAsync(IMessageChannel channel, IUser user)
    {
        var drops = await _scraper.GetTwitchDropsAsync();
        await channel.SendMessageAsync(string.IsNullOrWhiteSpace(drops)
            ? "Сейчас нет активных Twitch Drops."
            : $"Актуальные Twitch Drops:\n{drops}");
    }

    public ApplicationCommandProperties RegisterSlashCommand()
    {
        return new SlashCommandBuilder()
            .WithName("twitchdrops")
            .WithDescription("Получить информацию о текущих Twitch Drops")
            .Build();
    }
}
