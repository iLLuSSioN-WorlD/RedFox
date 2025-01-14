using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class TwitchDropsCommand : ICommand
    {
        private readonly TwitchDropsService _twitchDropsService;

        public TwitchDropsCommand(TwitchDropsService twitchDropsService)
        {
            _twitchDropsService = twitchDropsService;
        }

        public string CommandName => "twitchdrops";

        // Обработка текстовой команды
        public async Task ExecuteAsync(IMessageChannel channel, IUser user)
        {
            var drops = await _twitchDropsService.GetActiveDropsAsync();

            var message = string.IsNullOrWhiteSpace(drops)
                ? "Сейчас нет активных Twitch Drops."
                : $"Актуальные Twitch Drops:\n{drops}";

            await channel.SendMessageAsync(message);
        }

        // Обработка слэш-команды
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            var drops = await _twitchDropsService.GetActiveDropsAsync();

            var message = string.IsNullOrWhiteSpace(drops)
                ? "Сейчас нет активных Twitch Drops."
                : $"Актуальные Twitch Drops:\n{drops}";

            await command.RespondAsync(message);
        }
    }
}
