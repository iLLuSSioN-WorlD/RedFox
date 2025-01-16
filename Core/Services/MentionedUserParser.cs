using Discord;
using System;
using System.Text.RegularExpressions;

namespace RedFox.Core.Services
{
    public class MentionedUserParser
    {
        // Метод для извлечения упомянутого пользователя
        public static ulong? ExtractUserId(string mentionedUser)
        {
            // Регулярное выражение для извлечения ID пользователя из упоминания
            var match = Regex.Match(mentionedUser, @"<@!?(\d+)>");

            if (match.Success)
            {
                // Преобразуем ID в ulong
                if (ulong.TryParse(match.Groups[1].Value, out ulong userId))
                {
                    return userId;
                }
            }

            return null; // Возвращаем null, если не удалось извлечь ID
        }
    }
}
