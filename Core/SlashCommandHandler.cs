using Discord;
using Discord.Net;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class SlashCommandHandler
    {
        private readonly DiscordSocketClient _client;

        public SlashCommandHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task RegisterCommandsAsync()
        {
            var commands = new List<ApplicationCommandProperties>
            {
                // Регистрация команды /ping
                new SlashCommandBuilder()
                    .WithName("ping")
                    .WithDescription("Ответить Pong!")
                    .Build(),

                // Регистрация команды /roll
                new SlashCommandBuilder()
                    .WithName("roll")
                    .WithDescription("Сгенерировать случайное число от 1 до 100")
                    .Build()
            };

            try
            {
                // Используем Rest для глобальной регистрации
                await _client.BulkOverwriteGlobalApplicationCommandsAsync(commands.ToArray());
                Console.WriteLine("Слэш-команды успешно зарегистрированы.");
            }
            catch (HttpException ex)
            {
                Console.WriteLine($"Ошибка при регистрации команд: {ex.Message}");
            }
        }
    }
}
