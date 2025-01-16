using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    public class RpsCommand : ICommand
    {
        private readonly DiscordSocketClient _client;

        public RpsCommand(DiscordSocketClient client)
        {
            _client = client;
        }

        public string CommandName => "rps";

        private static readonly string[] Choices = { "камень", "ножницы", "бумага" };

        // Словарь для хранения активных вызовов (ключ — вызывающий игрок, значение — вызываемый игрок)
        private static readonly Dictionary<ulong, ulong> DuelRequests = new Dictionary<ulong, ulong>();

        // Словарь для хранения выборов игроков
        private static readonly Dictionary<ulong, string> PlayerChoices = new Dictionary<ulong, string>();

        // Метод обработки текстовых команд
        public async Task ExecuteAsync(IMessageChannel channel, IUser user, string[] args)
        {
            if (args.Length < 1)
            {
                await channel.SendMessageAsync("Пожалуйста, укажите игрока для дуэли.");
                return;
            }

            var mentionedUser = ExtractUserFromMention(args[0]);
            if (mentionedUser == null || mentionedUser.Id == user.Id)
            {
                await channel.SendMessageAsync("Вы не можете вызвать самого себя на дуэль.");
                return;
            }

            // Проверяем, есть ли активный вызов на дуэль между этими игроками
            if (DuelRequests.ContainsKey(user.Id))
            {
                await channel.SendMessageAsync($"Вы уже вызвали {mentionedUser.Mention} на дуэль. Ожидайте ответа.");
                return;
            }

            if (DuelRequests.ContainsValue(user.Id))
            {
                await channel.SendMessageAsync("Вы не можете вызывать других игроков, пока не завершили текущую дуэль.");
                return;
            }

            // Добавляем вызов в список активных дуэлей
            DuelRequests[user.Id] = mentionedUser.Id;
            await channel.SendMessageAsync($"{mentionedUser.Mention}, {user.Mention} вызвал вас на дуэль! Напишите `/rps принять`, чтобы принять вызов.");
        }

        // Метод обработки слэш-команд
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            var mentionedUser = command.Data.Options.FirstOrDefault(o => o.Name == "user")?.Value as SocketUser;
            var userChoice = command.Data.Options.FirstOrDefault(o => o.Name == "choice")?.Value?.ToString().ToLower();

            if (mentionedUser == null)
            {
                await command.RespondAsync("Укажите игрока для дуэли.");
                return;
            }

            if (mentionedUser.Id == command.User.Id)
            {
                await command.RespondAsync("Вы не можете вызвать самого себя на дуэль.");
                return;
            }

            if (DuelRequests.ContainsKey(command.User.Id))
            {
                await command.RespondAsync($"Вы уже вызвали {mentionedUser.Mention} на дуэль. Ожидайте его ответа.");
                return;
            }

            if (DuelRequests.ContainsValue(command.User.Id))
            {
                await command.RespondAsync("Вы не можете вызывать других игроков, пока не завершили текущую дуэль.");
                return;
            }

            // Добавляем вызов в список активных дуэлей
            DuelRequests[command.User.Id] = mentionedUser.Id;
            await command.RespondAsync($"{mentionedUser.Mention}, {command.User.Mention} вызвал вас на дуэль! Напишите `/rps принять`, чтобы принять вызов.");
        }

        // Метод для принятия вызова
        public async Task AcceptDuelAsync(SocketSlashCommand command)
        {
            if (!DuelRequests.ContainsValue(command.User.Id))
            {
                await command.RespondAsync("Вы не получили вызов на дуэль или уже приняли его.");
                return;
            }

            // Находим игрока, вызвавшего текущего пользователя
            var challenger = DuelRequests.FirstOrDefault(kvp => kvp.Value == command.User.Id).Key;

            // Удаляем вызов из списка
            DuelRequests.Remove(challenger);

            await command.RespondAsync($"{command.User.Mention} принял вызов от {challenger}. Выберите: камень, ножницы или бумага.");
        }

        // Метод для извлечения пользователя из упоминания
        private IUser ExtractUserFromMention(string mention)
        {
            if (mention.StartsWith("<@") && mention.EndsWith(">"))
            {
                var userIdString = mention.Trim('<', '@', '>');
                if (ulong.TryParse(userIdString, out ulong userId))
                {
                    return _client.GetUser(userId);
                }
            }

            return null;
        }

        // Метод для определения результата игры
        private string GetResult(string userChoice, string opponentChoice)
        {
            if (userChoice == opponentChoice)
                return "Ничья!";

            if ((userChoice == "камень" && opponentChoice == "ножницы") ||
                (userChoice == "ножницы" && opponentChoice == "бумага") ||
                (userChoice == "бумага" && opponentChoice == "камень"))
                return "Ты выиграл!";

            return "Ты проиграл!";
        }

        // Регистрация слэш-команды
        public ApplicationCommandProperties RegisterSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("rps")
                .WithDescription("Игра Камень, Ножницы, Бумага.")
                .AddOption("user", ApplicationCommandOptionType.User, "Выберите оппонента для дуэли.", true)
                .AddOption("choice", ApplicationCommandOptionType.String, "Выберите камень, ножницы или бумагу.", false)
                .Build();
        }
    }
}
