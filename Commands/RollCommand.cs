using Discord;
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

        public async Task ExecuteAsync(IMessageChannel channel, IUser user)
        {
            // Генерируем случайное число от 1 до 100
            var randomNumber = _randomService.Generate(1, 100);

            // Конвертируем число в эмодзи с заданным количеством цифр (3)
            var emojiString = _emojiConverter.ConvertNumberToEmoji(randomNumber, 3);

            // Формируем сообщение с результатом
            var message = $"<@{user.Id}> получает случайное число (1-100): {emojiString}";

            // Отправляем сообщение в канал
            await channel.SendMessageAsync(message);
        }
    }
}
