using Discord.WebSocket;
using Discord;
using DiscordBot;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        var configManager = new ConfigManager(); // Загружаем основную конфигурацию
        var twitchConfigManager = new TwitchConfigManager();
        var twitchConfig = twitchConfigManager.GetConfig();

        // Проверяем, корректно ли настроен Twitch API
        if (string.IsNullOrWhiteSpace(twitchConfig.ClientId) || string.IsNullOrWhiteSpace(twitchConfig.ClientSecret))
        {
            Console.WriteLine("Twitch API отключен: Проверьте настройки в twitch.json.");
        }
        else
        {
            var twitchAuthService = new TwitchAuthService(twitchConfigManager);

            try
            {
                // Пытаемся обновить токен Twitch
                await twitchAuthService.AuthenticateAsync();
                Console.WriteLine("Twitch API: Токен успешно обновлен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка авторизации Twitch API: {ex.Message}");
                Console.WriteLine("Продолжаем работу без Twitch API.");
            }
        }

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
            .AddSingleton<ICommand, PingCommand>()
            .AddSingleton<ICommand, RollCommand>()
            .AddSingleton<ICommand, TwitchDropsCommand>()
            .AddSingleton<RandomNumberService>()
            .AddSingleton<EmojiConverterService>()
            .AddSingleton(configManager.Config)
            .AddSingleton(new HttpClient())
            .AddSingleton<TwitchDropsScraper>()
            .BuildServiceProvider();

        var bot = serviceProvider.GetRequiredService<CommandHandler>();

        client.Log += LogAsync;

        await bot.InitializeAsync();
        await Task.Delay(-1); // Бесконечное ожидание, чтобы бот оставался онлайн
    }

    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }
}
