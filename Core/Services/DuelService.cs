using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace RedFox.Core.Services
{
    public class DuelService
    {
        private readonly ConcurrentDictionary<ulong, ulong> _duelRequests = new();
        private readonly ConcurrentDictionary<ulong, IUserMessage> _requestMessages = new();
        private readonly DiscordSocketClient _client;

        public DuelService(DiscordSocketClient client)
        {
            _client = client;
            _client.ReactionAdded += OnReactionAddedAsync;
        }

        public async Task<bool> CreateDuelAsync(IMessageChannel channel, IUser challenger, IUser challenged)
        {
            // Проверяем, есть ли уже активные вызовы от или для данного пользователя
            if (_duelRequests.ContainsKey(challenger.Id) || _duelRequests.Values.Contains(challenger.Id))
            {
                return false;
            }

            // Сохраняем вызов
            _duelRequests[challenger.Id] = challenged.Id;

            // Отправляем сообщение вызова
            var message = await channel.SendMessageAsync($"{challenged.Mention}, {challenger.Mention} вызвал вас на дуэль! Нажмите 👍, чтобы принять, или 👎, чтобы отклонить.");
            _requestMessages[challenger.Id] = message;

            // Добавляем реакции
            try
            {
                await message.AddReactionAsync(new Emoji("👍"));
                await message.AddReactionAsync(new Emoji("👎"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении реакций: {ex.Message}");
            }

            return true;
        }

        public IUser ExtractUserFromMention(string mention)
        {
            try
            {
                if (mention.StartsWith("<@") && mention.EndsWith(">"))
                {
                    var userIdString = mention.Trim('<', '@', '!');
                    if (ulong.TryParse(userIdString, out ulong userId))
                    {
                        return _client.GetUser(userId);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в ExtractUserFromMention: {ex.Message}");
            }

            return null;
        }

        private async Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> cache, Cacheable<IMessageChannel, ulong> channelCache, SocketReaction reaction)
        {
            try
            {
                // Проверяем, существует ли сообщение
                var channel = await channelCache.GetOrDownloadAsync();
                var message = await cache.GetOrDownloadAsync();

                if (message == null || channel == null) return;

                // Проверяем, является ли сообщение частью активной дуэли
                var challengerId = _duelRequests.FirstOrDefault(x => _requestMessages[x.Key].Id == message.Id).Key;
                if (challengerId == 0) return;

                var challengedId = _duelRequests[challengerId];

                // Игнорируем реакции от пользователей, кроме вызванного игрока
                if (reaction.UserId != challengedId) return;

                // Обработка реакций 👍 или 👎
                if (reaction.Emote.Name == "👍")
                {
                    await channel.SendMessageAsync($"Дуэль началась между <@{challengerId}> и <@{challengedId}>! Сделайте выбор: `камень`, `ножницы` или `бумага`.");
                }
                else if (reaction.Emote.Name == "👎")
                {
                    await channel.SendMessageAsync($"<@{challengedId}> отклонил вызов от <@{challengerId}>.");
                }

                // Удаляем дуэль из активных
                _duelRequests.TryRemove(challengerId, out _);
                _requestMessages.TryRemove(challengerId, out _);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в OnReactionAddedAsync: {ex.Message}");
            }
        }

    }
}
