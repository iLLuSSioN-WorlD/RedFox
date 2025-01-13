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
                .AddSingleton<CommandHandler>()
                .AddSingleton<ICommand, PingCommand>()
                .AddSingleton<ICommand, RollCommand>() // Регистрируем команду Roll
                .AddSingleton<RandomNumberService>() // Регистрируем сервис рандома
                .AddSingleton<EmojiConverterService>() // Регистрируем сервис конвертации в смайлики
                .AddSingleton(configManager.Config) // Регистрируем конфигурацию в DI
                .AddSingleton<SlashCommandHandler>() // Регистрация обработчика слэш-команд
                .AddSingleton<InteractionHandler>()  // Регистрация обработчика взаимодействий
                .BuildServiceProvider();

            var bot = serviceProvider.GetRequiredService<CommandHandler>();
            var slashCommandHandler = serviceProvider.GetRequiredService<SlashCommandHandler>();
            var interactionHandler = serviceProvider.GetRequiredService<InteractionHandler>();

            client.Log += LogAsync;

            client.Ready += async () =>
            {
                Console.WriteLine("Клиент готов. Регистрируем слэш-команды...");
                await slashCommandHandler.RegisterCommandsAsync();
            };

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
