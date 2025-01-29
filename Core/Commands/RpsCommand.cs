using Discord;
using Discord.WebSocket;
using RedFox.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    public class RpsCommand : ICommand
    {
        private readonly DiscordSocketClient _client;
        private readonly DuelService _duelService;
        private readonly ConcurrentDictionary<ulong, ulong> _duelRequests;

        public string CommandName => "rps";

        // Конструктор
        public RpsCommand(DiscordSocketClient client, DuelService duelService)
        {
            _client = client;
            _duelService = duelService;
            _duelRequests = new ConcurrentDictionary<ulong, ulong>();

            // Подписка на событие добавления реакции
            _client.ReactionAdded += OnReactionAddedAsync;
        }

        // Метод для запуска команды /rps
        public async Task ExecuteAsync(IMessageChannel channel, IUser user, string[] args)
        {
            try
            {
                // Проверяем, что аргументы есть
                if (args.Length < 1)
                {
                    await channel.SendMessageAsync("Пожалуйста, укажите игрока для дуэли.");
                    return;
                }

                // Извлекаем пользователя из упоминания
                var mentionedUser = _duelService.ExtractUserFromMention(args[0]);
                if (mentionedUser == null)
                {
                    await channel.SendMessageAsync("Не удалось определить пользователя. Убедитесь, что вы правильно указали оппонента.");
                    return;
                }

                // Проверяем, что пользователь не вызывает сам себя
                if (mentionedUser.Id == user.Id)
                {
                    await channel.SendMessageAsync("Вы не можете вызвать самого себя на дуэль.");
                    return;
                }

                // Создаем дуэль
                var result = await _duelService.CreateDuelAsync(channel, user, mentionedUser);

                if (!result)
                {
                    await channel.SendMessageAsync($"<@{user.Id}>, вы уже участвуете в активной дуэли или ожидаете ответа.");
                }
                else
                {
                    // Отправляем сообщение и добавляем реакции
                    var message = await channel.SendMessageAsync($"{mentionedUser.Mention}, {user.Mention} вызвал вас на дуэль! Нажмите 👍 для принятия, или 👎 для отклонения.");

                    // Добавляем реакции на сообщение
                    await message.AddReactionAsync(new Emoji("👍"));
                    await message.AddReactionAsync(new Emoji("👎"));
                }
            }
            catch (Exception ex)
            {
                await channel.SendMessageAsync("Произошла ошибка при обработке команды. Проверьте логи.");
                Console.WriteLine($"Ошибка в RpsCommand.ExecuteAsync: {ex.Message}");
            }
        }

        // Метод обработки реакции на сообщение
        private async Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> cache, Cacheable<IMessageChannel, ulong> channelCache, SocketReaction reaction)
        {
            try
            {
                var channel = await channelCache.GetOrDownloadAsync();
                var message = await cache.GetOrDownloadAsync();

                // Проверяем, что сообщение и канал существуют
                if (message == null || channel == null) return;

                // Проверяем, является ли реакция одной из нужных
                if (reaction.Emote.Name != "👍" && reaction.Emote.Name != "👎") return;

                // Проверяем, кто добавил реакцию (только вызываемый пользователь)
                if (reaction.UserId == message.Author.Id)
                {
                    if (reaction.Emote.Name == "👍")
                    {
                        // Принятие дуэли
                        await channel.SendMessageAsync($"Дуэль началась между <@{message.Author.Id}> и <@{reaction.UserId}>! Выберите камень, ножницы или бумагу.");
                    }
                    else if (reaction.Emote.Name == "👎")
                    {
                        // Отклонение дуэли
                        await channel.SendMessageAsync($"<@{reaction.UserId}> отклонил вызов от <@{message.Author.Id}>.");
                    }

                    // Удаляем дуэль из списка запросов
                    _duelRequests.TryRemove(message.Author.Id, out _);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке реакции: {ex.Message}");
            }
        }

        // Метод для регистрации слэш-команды
        public ApplicationCommandProperties RegisterSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("rps")
                .WithDescription("Вызвать игрока на дуэль Камень-Ножницы-Бумага")
                .AddOption("user", ApplicationCommandOptionType.User, "Игрок для вызова", isRequired: true)
                .Build();
        }

        // Метод для обработки слэш-команды
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            var userOption = command.Data.Options.FirstOrDefault(opt => opt.Name == "user")?.Value as SocketUser;
            if (userOption == null)
            {
                await command.RespondAsync("Пожалуйста, укажите игрока для дуэли.");
                return;
            }

            var user = command.User;

            // Проверяем, что пользователь не вызывает сам себя
            if (userOption.Id == user.Id)
            {
                await command.RespondAsync("Вы не можете вызвать самого себя на дуэль.");
                return;
            }

            // Создаем дуэль
            var result = await _duelService.CreateDuelAsync(command.Channel, user, userOption);

            if (!result)
            {
                await command.RespondAsync($"<@{user.Id}>, вы уже участвуете в активной дуэли или ожидаете ответа.");
            }
            else
            {
                await command.RespondAsync($"Вы вызвали <@{userOption.Id}> на дуэль! Ожидайте его ответа.");
            }
        }

        // Реализуем метод ExecuteComponentCommandAsync для обработки нажатий на кнопки
        public async Task ExecuteComponentCommandAsync(SocketMessageComponent component)
        {
            try
            {
                // Проверяем, что взаимодействие с кнопкой от правильного пользователя
                if (component.User.IsBot)
                    return;

                // Логика для обработки нажатий на кнопки
                if (component.Data.CustomId == "accept_duel")
                {
                    await component.RespondAsync("Дуэль принята!");
                    // Тут можно добавить логику начала игры
                }
                else if (component.Data.CustomId == "decline_duel")
                {
                    await component.RespondAsync("Дуэль отклонена.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке компонента: {ex.Message}");
            }
        }
    }
}
