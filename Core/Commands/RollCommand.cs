using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class RollCommand : ICommand
    {
        private readonly RandomNumberService _randomService;
        private readonly EmojiConverterService _emojiConverter;

        public RollCommand(RandomNumberService randomService, EmojiConverterService emojiConverter)
        {
            _randomService = randomService;
            _emojiConverter = emojiConverter;
        }

        public string CommandName => "roll";

        public async Task ExecuteAsync(IMessageChannel channel, IUser user, string[] args)
        {
            int minRollLimit = 1;
            int maxRollLimit = 100;

            // Использование switch для обработки аргументов
            switch (args.Length)
            {
                case 1 when int.TryParse(args[0], out var maxLimit):
                    // Один аргумент — максимальное значение
                    maxRollLimit = maxLimit;
                    break;

                case 2 when int.TryParse(args[0], out var minLimit) && int.TryParse(args[1], out var maxLimit):
                    // Два аргумента — минимальное и максимальное значение
                    minRollLimit = minLimit;
                    maxRollLimit = maxLimit;
                    break;

                // В default случае остаются значения по умолчанию (minRollLimit = 1, maxRollLimit = 100)
                default:
                    break;
            }

            // Проверка на корректность диапазона
            if (minRollLimit >= maxRollLimit)
            {
                await channel.SendMessageAsync("Ошибка: минимальное значение не может быть больше или равно максимальному.");
                return;
            }

            var randomNumber = _randomService.Generate(minRollLimit, maxRollLimit);
            var emojiString = _emojiConverter.ConvertNumberToEmoji(randomNumber, 3);
            var message = $"<@{user.Id}> получает случайное число ({minRollLimit}-{maxRollLimit}): {emojiString}";
            await channel.SendMessageAsync(message);
        }

        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            long minRollLimit = 1;  // Используем long для минимального лимита
            long maxRollLimit = 100;  // Используем long для максимального лимита

            // Получаем параметры из команды
            var args = command.Data.Options;

            // Использование switch для обработки аргументов
            switch (args.Count)
            {
                case 1 when args.ElementAt(0).Type == ApplicationCommandOptionType.Integer:
                    // Проверяем, что значение действительно типа Int64
                    maxRollLimit = (long)args.ElementAt(0).Value;
                    break;

                case 2 when args.ElementAt(0).Type == ApplicationCommandOptionType.Integer && args.ElementAt(1).Type == ApplicationCommandOptionType.Integer:
                    // Проверяем оба значения
                    minRollLimit = (long)args.ElementAt(0).Value;
                    maxRollLimit = (long)args.ElementAt(1).Value;
                    break;

                // В default случае остаются значения по умолчанию
                default:
                    break;
            }

            // Проверяем, что minRollLimit меньше maxRollLimit
            if (minRollLimit >= maxRollLimit)
            {
                await command.RespondAsync("Ошибка: минимальное значение не может быть больше или равно максимальному.");
                return;
            }

            // Если нужно преобразовать long в int, делаем явное приведение с проверкой диапазона
            if (minRollLimit < int.MinValue || minRollLimit > int.MaxValue || maxRollLimit < int.MinValue || maxRollLimit > int.MaxValue)
            {
                await command.RespondAsync("Диапазон слишком большой. Пожалуйста, укажите более разумные значения.");
                return;
            }

            // Преобразуем значения minRollLimit и maxRollLimit в int и генерируем случайное число
            var randomNumber = _randomService.Generate((int)minRollLimit, (int)maxRollLimit);
            var emojiString = _emojiConverter.ConvertNumberToEmoji(randomNumber, 3);

            var message = $"Ваше случайное число ({minRollLimit}-{maxRollLimit}): {emojiString}";
            await command.RespondAsync(message);
        }

        public ApplicationCommandProperties RegisterSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("roll")
                .WithDescription("Сгенерировать случайное число в указанном диапазоне")
                .AddOption("min", ApplicationCommandOptionType.Integer, "Минимальное значение", false)
                .AddOption("max", ApplicationCommandOptionType.Integer, "Максимальное значение", false)
                .Build();
        }
    }
}
