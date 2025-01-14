using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace DiscordBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configManager = new ConfigManager(); // Загружаем основную конфигурацию
            var twitchConfigManager = new TwitchConfigManager(); // Создаём менеджер для Twitch
            var twitchConfig = twitchConfigManager.LoadConfig(TwitchConfigManager.ConfigFilePath); // Загружаем конфигурацию Twitch

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
                .AddSingleton<ICommand, TwitchDropsCommand>() // Регистрация команды Twitch Drops
                .AddSingleton<RandomNumberService>()
                .AddSingleton<EmojiConverterService>()
                .AddSingleton(configManager.Config)
                .AddSingleton(twitchConfig) // Передаём TwitchConfig
                .AddSingleton(new HttpClient()) // Передаём HttpClient
                .AddSingleton(new TwitchAuthService(new HttpClient(), TwitchConfigManager.ConfigFilePath)) // Передаём путь к twitch.json
                .AddSingleton<TwitchDropsService>()
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
}
