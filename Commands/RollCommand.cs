using Discord;
using Discord.WebSocket;
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
            var randomNumber = _randomService.Generate(1, 100);
            var emojiString = _emojiConverter.ConvertNumberToEmoji(randomNumber, 3);
            var message = $"<@{user.Id}> получает случайное число (1-100): {emojiString}";
            await channel.SendMessageAsync(message);
        }

        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            var randomNumber = _randomService.Generate(1, 100);
            var emojiString = _emojiConverter.ConvertNumberToEmoji(randomNumber, 3);
            var message = $"Ваше случайное число (1-100): {emojiString}";
            await command.RespondAsync(message);
        }

        public ApplicationCommandProperties RegisterSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("roll")
                .WithDescription("Сгенерировать случайное число от 1 до 100")
                .Build();
        }
    }
}
