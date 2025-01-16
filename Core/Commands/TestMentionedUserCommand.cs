using Discord;
using Discord.WebSocket;
using RedFox.Core.Services;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    public class TestMentionedUserCommand : ICommand
    {
        public string CommandName => "testmention";

        // Для текстовых команд
        public async Task ExecuteAsync(IMessageChannel channel, IUser user, string[] args)
        {
            if (args.Length == 0)
            {
                await channel.SendMessageAsync($"Привет, <@{user.Id}>! Пожалуйста, упомяни кого-нибудь в формате `@User`.");
                return;
            }

            var mentionedUser = args[0];
            var userId = MentionedUserParser.ExtractUserId(mentionedUser);

            if (userId.HasValue)
            {
                await channel.SendMessageAsync($"Упомянутый пользователь: {mentionedUser} имеет ID: {userId.Value}");
            }
            else
            {
                await channel.SendMessageAsync("Неверный формат упоминания. Убедись, что используешь правильный формат, например: `@User`.");
            }
        }

        // Для слэш-команд
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            var user = command.User;
            var mentionedUserOption = command.Data.Options.FirstOrDefault()?.Value;

            if (mentionedUserOption == null || !(mentionedUserOption is SocketUser mentionedUser))
            {
                await command.RespondAsync($"Привет, <@{user.Id}>! Для тестирования упомяни кого-то в формате `@User`.");
                return;
            }

            var userId = MentionedUserParser.ExtractUserId(mentionedUser.Mention);

            if (userId.HasValue)
            {
                await command.RespondAsync($"Упомянутый пользователь: {mentionedUser.Mention} имеет ID: {userId.Value}");
            }
            else
            {
                await command.RespondAsync("Неверный формат упоминания. Убедись, что используешь правильный формат, например: `@User`.");
            }
        }

        // Регистрация слэш-команды
        public ApplicationCommandProperties RegisterSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("testmention")
                .WithDescription("Тестирует упомянутого пользователя и извлекает его ID")
                .AddOption("user", ApplicationCommandOptionType.User, "Упомянутый пользователь для теста", isRequired: true)
                .Build();
        }
    }
}
