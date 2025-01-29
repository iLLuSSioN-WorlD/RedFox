using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Services;

namespace DiscordBot.Commands
{
    public class DiceCommand : ICommand
    {
        private readonly DiceService _diceService = new();

        public string CommandName => "dice";

        // Реализация метода ExecuteAsync для команд с префиксом
        public async Task ExecuteAsync(IMessageChannel channel, IUser user, string[] args)
        {
            int sides = 6;  // по умолчанию 6-гранный кубик
            IUser? opponent = null;

            // Проверка аргументов
            if (args.Length > 0)
            {
                // Обработка первого аргумента для граней кубика
                if (int.TryParse(args[0], out int parsedSides))
                {
                    // Проверка, что количество граней одно из допустимых
                    if (parsedSides == 4 || parsedSides == 6 || parsedSides == 8 || parsedSides == 10 || parsedSides == 12 || parsedSides == 20)
                    {
                        sides = parsedSides;
                    }
                }
            }

            // Проверка второго аргумента для нахождения соперника
            if (args.Length > 1)
            {
                string userIdString = args[1].Trim('<', '@', '!', '>');
                if (ulong.TryParse(userIdString, out ulong userId))
                {
                    opponent = await channel.GetUserAsync(userId);
                }
            }

            // Если соперника нет, просто бросаем кубик
            if (opponent == null)
            {
                int result = _diceService.Roll(sides);
                await channel.SendMessageAsync($"🎲 {user.Mention} бросает `{sides}-гранный кубик` и получает `{result}`");
            }
            else
            {
                // Если соперник есть, бросаем два кубика и определяем победителя
                int authorRoll = _diceService.Roll(sides);
                int opponentRoll = _diceService.Roll(sides);
                string outcome = authorRoll > opponentRoll ? "побеждает" : authorRoll < opponentRoll ? "проигрывает" : "ничья";

                // Форматированный вывод без экранирования "}"
                await channel.SendMessageAsync(
                    $"🎲 {user.Mention} вызвал {opponent.Mention} на бросок `{sides}-гранного кубика`!\n\n" +
                    $"🎲 {user.Mention} бросает и получает `{authorRoll}`\n" +
                    $"🎲 {opponent.Mention} бросает и получает `{opponentRoll}`\n\n" +
                    $"🏆 {user.Mention} {outcome}!"
                );
            }
        }

        // Реализация метода ExecuteSlashCommandAsync для Slash-команд
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            int sides = 6;  // по умолчанию 6-гранный кубик
            IUser? opponent = null;
            var options = command.Data.Options;

            // Парсим параметр sides, если он передан
            foreach (var option in options)
            {
                if (option.Name == "sides" && option.Value is long sidesValue)
                {
                    sides = (int)sidesValue; // Преобразуем значение в int
                }
                else if (option.Name == "opponent" && option.Value is IUser user)
                {
                    opponent = user;
                }
            }

            // Проверка валидности кубика
            if (sides != 4 && sides != 6 && sides != 8 && sides != 10 && sides != 12 && sides != 20)
            {
                await command.RespondAsync("❌ Недопустимое количество граней! Поддерживаемые значения: 4, 6, 8, 10, 12, 20.");
                return;
            }

            // Если соперника нет, просто бросаем кубик
            if (opponent == null)
            {
                int result = _diceService.Roll(sides);
                await command.RespondAsync($"🎲 {command.User.Mention} бросает `{sides}-гранный кубик` и получает `{result}`");
            }
            else
            {
                // Если соперник есть, бросаем два кубика и определяем победителя
                int authorRoll = _diceService.Roll(sides);
                int opponentRoll = _diceService.Roll(sides);
                string outcome = authorRoll > opponentRoll ? "побеждает" : authorRoll < opponentRoll ? "проигрывает" : "ничья";

                // Форматированный вывод без экранирования "}"
                await command.RespondAsync(
                    $"🎲 {command.User.Mention} вызвал {opponent.Mention} на бросок `{sides}-гранного кубика`!\n\n" +
                    $"🎲 {command.User.Mention} бросает и получает `{authorRoll}`\n" +
                    $"🎲 {opponent.Mention} бросает и получает `{opponentRoll}`\n\n" +
                    $"🏆 {command.User.Mention} {outcome}!"
                );
            }
        }

        // Регистрируем команду в Slash-командах
        public ApplicationCommandProperties RegisterSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("dice")
                .WithDescription("Бросить кубик с заданным количеством граней")
                .AddOption("sides", ApplicationCommandOptionType.Integer, "Количество граней кубика", false) // Сторона кубика по умолчанию, необязательна
                .AddOption("opponent", ApplicationCommandOptionType.User, "Соперник для PvP броска", false) // Соперник для PvP
                .Build();
        }
    }
}
