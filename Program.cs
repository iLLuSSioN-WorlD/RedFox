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

            var serviceProvider = new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
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

            await bot.InitializeAsync();
            //await slashCommandHandler.RegisterCommandsAsync();

            await Task.Delay(-1);
        }
    }
}
