using System.Collections.Generic;

namespace DiscordBot
{
    public class EmojiConverterService
    {
        private readonly Dictionary<char, string> _emojiMapping = new()
        {
            { '0', ":zero:" },
            { '1', ":one:" },
            { '2', ":two:" },
            { '3', ":three:" },
            { '4', ":four:" },
            { '5', ":five:" },
            { '6', ":six:" },
            { '7', ":seven:" },
            { '8', ":eight:" },
            { '9', ":nine:" }
        };

        public string ConvertNumberToEmoji(int number, int? totalDigits = null)
        {
            // Определяем количество цифр для форматирования
            var numberString = number.ToString();
            var formattedNumber = totalDigits.HasValue
                ? number.ToString($"D{totalDigits.Value}") // Форматируем с ведущими нулями
                : numberString; // Используем длину числа

            var result = "";

            // Конвертируем каждую цифру в эмодзи
            foreach (var digit in formattedNumber)
            {
                if (_emojiMapping.TryGetValue(digit, out var emoji))
                {
                    result += emoji;
                }
            }

            return result;
        }
    }
}
