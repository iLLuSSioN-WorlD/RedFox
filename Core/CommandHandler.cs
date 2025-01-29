using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IEnumerable<ICommand> _commands;
        private readonly Config _config;

        public CommandHandler(DiscordSocketClient client, IEnumerable<ICommand> commands, Config config)
        {
            _client = client;
            _commands = commands;
            _config = config;

            _client.MessageReceived += HandleTextCommandAsync;
            _client.SlashCommandExecuted += HandleSlashCommandAsync;
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

        private async Task HandleTextCommandAsync(SocketMessage message)
        {
            if (message is not SocketUserMessage userMessage) return;

            int argPos = 0;
            if (!userMessage.HasCharPrefix('/', ref argPos)) return; // Проверяем, начинается ли команда с "/"

            var commandContent = userMessage.Content.Substring(argPos);
            var commandName = commandContent.Split(' ')[0].ToLower();
            var args = commandContent.Substring(commandName.Length).Trim().Split(' ');

            var command = _commands.FirstOrDefault(c => c.CommandName == commandName);

            if (command != null)
            {
                await command.ExecuteAsync(userMessage.Channel, userMessage.Author, args);
            }
            else
            {
                await userMessage.Channel.SendMessageAsync($"Команда `{commandName}` не найдена.");
            }
        }

        private async Task HandleSlashCommandAsync(SocketSlashCommand command)
        {
            var matchingCommand = _commands.FirstOrDefault(c => c.CommandName == command.CommandName);

            if (matchingCommand != null)
            {
                await matchingCommand.ExecuteSlashCommandAsync(command);
            }
            else
            {
                await command.RespondAsync($"Команда `{command.CommandName}` не найдена.", ephemeral: true);
            }
        }

        private async Task RegisterCommandsAsync()
        {
            var commands = new List<ApplicationCommandProperties>();

            foreach (var command in _commands)
            {
                var slashCommand = command.RegisterSlashCommand();
                if (slashCommand != null)
                {
                    commands.Add(slashCommand);
                }
            }

            try
            {
                // Регистрируем или обновляем команды
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
