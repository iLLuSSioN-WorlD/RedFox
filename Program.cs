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
            var twitchConfigManager = new TwitchConfigManager();
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
                .AddSingleton<ICommand, TwitchDropsCommand>()
                .AddSingleton<RandomNumberService>()
                .AddSingleton<EmojiConverterService>()
                .AddSingleton(configManager.Config)
                .AddSingleton(twitchConfig) // Передаём объект TwitchConfig
                .AddSingleton(new HttpClient())
                .AddSingleton<TwitchConfigManager>() // Регистрируем менеджер конфигурации
                .AddSingleton<TwitchAuthService>()
                .AddSingleton<TwitchDropsService>()
                .AddSingleton<TwitchDropsScraper>()
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
