using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Net;

namespace DiscordBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly Config _config;

        public CommandHandler(DiscordSocketClient client, IServiceProvider services, Config config)
        {
            _client = client;
            _services = services;
            _config = config;

            // Подключаем обработчики событий
            _client.MessageReceived += HandleCommandAsync;
            _client.InteractionCreated += HandleInteractionAsync;
        }

        public async Task InitializeAsync()
        {
            if (string.IsNullOrEmpty(_config.Token))
            {
                throw new ArgumentNullException(nameof(_config.Token), "Токен не указан в конфигурации.");
            }

            // Регистрация события Ready для отсроченной регистрации команд
            _client.Ready += OnReadyAsync;

            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();
        }

        private async Task OnReadyAsync()
        {
            _client.Ready -= OnReadyAsync; // Убираем подписку после выполнения
            await RegisterCommandsAsync();
        }

        private async Task HandleCommandAsync(SocketMessage message)
        {
            if (message is not SocketUserMessage userMessage || message.Author.IsBot) return;

            if (!userMessage.Content.StartsWith(_config.Prefix)) return;

            var command = userMessage.Content[_config.Prefix.Length..].Split(' ')[0].ToLower();

            foreach (var service in _services.GetServices<ICommand>())
            {
                if (service.CommandName == command)
                {
                    await service.ExecuteAsync(userMessage.Channel, userMessage.Author);
                    return;
                }
            }

            await userMessage.Channel.SendMessageAsync("Неизвестная команда.");
        }

        private async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            if (interaction is not SocketSlashCommand slashCommand) return;

            foreach (var service in _services.GetServices<ICommand>())
            {
                if (service.CommandName == slashCommand.Data.Name)
                {
                    var command = service as dynamic;
                    await command.ExecuteSlashCommandAsync(slashCommand);
                    return;
                }
            }

            await slashCommand.RespondAsync("Неизвестная команда.", ephemeral: true);
        }


        private async Task RegisterCommandsAsync()
        {
            var commands = new List<ApplicationCommandProperties>
            {
                new SlashCommandBuilder()
                    .WithName("ping")
                    .WithDescription("Ответить Pong!")
                    .Build(),

                new SlashCommandBuilder()
                    .WithName("roll")
                    .WithDescription("Сгенерировать случайное число от 1 до 100")
                    .Build()
            };

            try
            {
                await _client.BulkOverwriteGlobalApplicationCommandsAsync(commands.ToArray());
                Console.WriteLine("Слэш-команды успешно зарегистрированы.");
            }
            catch (HttpException ex)
            {
                Console.WriteLine($"Ошибка при регистрации слэш-команд: {ex.Message}");
                if (ex.Errors != null)
                {
                    foreach (var error in ex.Errors)
                    {
                        Console.WriteLine($"Inner Error: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Общая ошибка при регистрации слэш-команд: {ex.Message}");
            }
        }
    }
}
