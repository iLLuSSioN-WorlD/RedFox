﻿using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class TwitchDropsCommand : ICommand
    {
        private readonly TwitchDropsScraper _scraper;

        public TwitchDropsCommand(TwitchDropsScraper scraper)
        {
            _scraper = scraper;
        }

        public string CommandName => "twitchdrops";

        public async Task ExecuteAsync(IMessageChannel channel, IUser user)
        {
            var drops = await _scraper.GetTwitchDropsAsync();
            await channel.SendMessageAsync(string.IsNullOrWhiteSpace(drops)
                ? "Сейчас нет активных Twitch Drops."
                : $"Актуальные Twitch Drops:\n{drops}");
        }

        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            var drops = await _scraper.GetTwitchDropsAsync();
            await command.RespondAsync(string.IsNullOrWhiteSpace(drops)
                ? "Сейчас нет активных Twitch Drops."
                : $"Актуальные Twitch Drops:\n{drops}");
        }

        public ApplicationCommandProperties RegisterSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("twitchdrops")
                .WithDescription("Получить информацию о текущих Twitch Drops")
                .Build();
        }
    }
}
