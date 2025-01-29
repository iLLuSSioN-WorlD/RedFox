using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Services;

namespace DiscordBot.Commands
{
    public class DiceCommand : ICommand
    {
        private readonly DiceService _diceService = new();
        private readonly DiceMessageService _diceMessageService = new();
        private readonly ErrorHandlingService _errorHandlingService = new();

        public string CommandName => "dice";

        // Получение DisplayName или Username
        private string GetUserName(IUser user)
        {
            return (user is SocketGuildUser guildUser) ? guildUser.DisplayName : user.Username;
        }

        // Проверка валидности кубика
        private async Task<bool> ValidateDiceSidesAsync(IMessageChannel channel, int sides)
        {
            if (!_diceService.IsValidSides(sides))
            {
                string validSidesList = _diceService.GetValidSidesList();
                await _errorHandlingService.SendErrorMessageAsync(channel, $"Недопустимое количество граней! Поддерживаемые значения: {validSidesList}.");
                return false;
            }
            return true;
        }

        // Реализация метода ExecuteAsync для команд с префиксом
        public async Task ExecuteAsync(IMessageChannel channel, IUser user, string[] args)
        {
            int sides = 6;  // по умолчанию 6-гранный кубик
            int numDice = 1; // по умолчанию 1 кубик
            IUser? opponent = null;

            // Проверка аргументов
            if (args.Length > 0 && int.TryParse(args[0], out int parsedSides) && _diceService.IsValidSides(parsedSides))
            {
                sides = parsedSides;
            }

            if (args.Length > 1 && int.TryParse(args[1], out int parsedNumDice))
            {
                numDice = parsedNumDice;
            }

            if (args.Length > 2)
            {
                string userIdString = args[2].Trim('<', '@', '!', '>');
                if (ulong.TryParse(userIdString, out ulong userId))
                {
                    opponent = await channel.GetUserAsync(userId);
                }
            }

            // Проверка валидности кубика
            if (!await ValidateDiceSidesAsync(channel, sides)) return;

            string userName = GetUserName(user);

            // Если соперника нет, просто бросаем кубики
            if (opponent == null)
            {
                int[] results = _diceService.RollMultiple(sides, numDice);
                string message = await _diceMessageService.GenerateMultipleRollMessageAsync(userName, sides, numDice, results);
                await channel.SendMessageAsync(message);
            }
            else
            {
                // Если соперник есть, бросаем два кубика и определяем победителя
                int[] authorRolls = _diceService.RollMultiple(sides, numDice);
                int[] opponentRolls = _diceService.RollMultiple(sides, numDice);
                string opponentName = GetUserName(opponent);

                int authorSum = authorRolls.Sum();
                int opponentSum = opponentRolls.Sum();
                string outcome = authorSum > opponentSum ? "побеждает" : authorSum < opponentSum ? "проигрывает" : "ничья";

                string message = await _diceMessageService.GeneratePvPMessageAsync(userName, opponentName, sides, numDice, authorRolls, opponentRolls, outcome);
                await channel.SendMessageAsync(message);
            }
        }

        // Реализация метода ExecuteSlashCommandAsync для Slash-команд
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            int sides = 6;  // по умолчанию 6-гранный кубик
            int numDice = 1; // по умолчанию 1 кубик
            IUser? opponent = null;
            var options = command.Data.Options;

            // Парсим параметры sides и numdice
            foreach (var option in options)
            {
                if (option.Name == "sides" && option.Value is long sidesValue)
                {
                    sides = (int)sidesValue;
                }
                else if (option.Name == "numdice" && option.Value is long numDiceValue)
                {
                    numDice = (int)numDiceValue;
                }
                else if (option.Name == "opponent" && option.Value is IUser user)
                {
                    opponent = user;
                }
            }

            // Проверка валидности кубика
            if (!await ValidateDiceSidesAsync(command.Channel, sides)) return;

            string userName = GetUserName(command.User);

            // Если соперника нет, просто бросаем кубики
            if (opponent == null)
            {
                int[] results = _diceService.RollMultiple(sides, numDice);
                string message = await _diceMessageService.GenerateMultipleRollMessageAsync(userName, sides, numDice, results);
                await command.RespondAsync(message);
            }
            else
            {
                // Если соперник есть, бросаем два кубика и определяем победителя
                int[] authorRolls = _diceService.RollMultiple(sides, numDice);
                int[] opponentRolls = _diceService.RollMultiple(sides, numDice);
                string opponentName = GetUserName(opponent);

                int authorSum = authorRolls.Sum();
                int opponentSum = opponentRolls.Sum();
                string outcome = authorSum > opponentSum ? "побеждает" : authorSum < opponentSum ? "проигрывает" : "ничья";

                string message = await _diceMessageService.GeneratePvPMessageAsync(userName, opponentName, sides, numDice, authorRolls, opponentRolls, outcome);
                await command.RespondAsync(message);
            }
        }

        public ApplicationCommandProperties RegisterSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("dice")
                .WithDescription("Бросить кубики с заданным количеством граней")
                .AddOption("sides", ApplicationCommandOptionType.Integer, "Количество граней кубика", false) // Сторона кубика по умолчанию, необязательна
                .AddOption("numdice", ApplicationCommandOptionType.Integer, "Количество кубиков для броска", false) // Параметр для количества кубиков
                .AddOption("opponent", ApplicationCommandOptionType.User, "Соперник для PvP броска", false) // Параметр для соперника
                .Build();
        }
    }
}
