using System.Collections.Generic;
using System.Linq;

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
            var formattedNumber = totalDigits.HasValue
                ? number.ToString($"D{totalDigits.Value}") // Форматируем с ведущими нулями
                : number.ToString(); // Используем длину числа

            // Конвертируем каждую цифру в эмодзи и добавляем пробелы
            var emojiList = formattedNumber
                .Select(digit => _emojiMapping.ContainsKey(digit) ? _emojiMapping[digit] : string.Empty)
                .Where(emoji => !string.IsNullOrEmpty(emoji)); // Убираем пустые значения

            return string.Join(" ", emojiList); // Добавляем пробелы между эмодзи
        }
    }
}
