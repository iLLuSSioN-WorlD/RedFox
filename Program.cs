using Discord.WebSocket;
using Discord;
using DiscordBot;
using Microsoft.Extensions.DependencyInjection;
using DiscordBot.Commands;
using Victoria;
using DiscordBot.Services;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var configManager = new ConfigManager(); // Загружаем основную конфигурацию

        var client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.DirectMessages |
                             GatewayIntents.MessageContent
        });

        var serviceProvider = new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton<CommandHandler>()
            .AddSingleton<RandomNumberService>()
            .AddSingleton<ICommand, PingCommand>()
            .AddSingleton<IDiceService, DiceService>()
            .AddSingleton<DiceMessageService>()
            .AddSingleton<ErrorHandlingService>()
            .AddSingleton<EmojiConverterService>()
            .AddSingleton(configManager.Config)
            .AddSingleton<ICommand, GamesCommand>()
            .AddSingleton<CoinCommand>()  // Теперь регистрируем как сервис
            .AddSingleton<DiceCommand>()
            .AddSingleton<RollCommand>()
            .AddSingleton<RpsCommand>()
            .BuildServiceProvider();

        var bot = serviceProvider.GetRequiredService<CommandHandler>();

        client.Log += LogAsync;

        await bot.InitializeAsync();
        await Task.Delay(-1);
    }

    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }
}
