using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configManager = new ConfigManager(); // Загружаем конфигурацию

            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds |
                                 GatewayIntents.GuildMessages |
                                 GatewayIntents.DirectMessages |
                                 GatewayIntents.MessageContent // Если нужно читать текст сообщений
            });

            var serviceProvider = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(configManager.Config) // Регистрируем конфигурацию в DI
                .AddSingleton<CommandHandler>()
                .AddSingleton<ICommand, PingCommand>()
                .AddSingleton<ICommand, RollCommand>() // Регистрируем команду Roll
                .AddSingleton<RandomNumberService>() // Регистрируем сервис рандома
                .AddSingleton<EmojiConverterService>() // Регистрируем сервис конвертации в смайлики
                
                .BuildServiceProvider();

            var bot = serviceProvider.GetRequiredService<CommandHandler>();

            client.Log += LogAsync;

            // Запускаем бота
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
