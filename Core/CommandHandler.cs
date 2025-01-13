using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

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

            // Подключаем обработчик событий
            _client.MessageReceived += HandleCommandAsync;
        }

        public async Task InitializeAsync()
        {
            if (string.IsNullOrEmpty(_config.Token))
            {
                throw new ArgumentNullException(nameof(_config.Token), "Токен не указан в конфигурации.");
            }

            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();
        }

        private async Task HandleCommandAsync(SocketMessage message)
        {
            if (message is not SocketUserMessage userMessage || message.Author.IsBot) return;

            // Проверяем, начинается ли сообщение с указанного префикса
            if (!userMessage.Content.StartsWith(_config.Prefix)) return;

            // Извлекаем команду
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
    }
}
