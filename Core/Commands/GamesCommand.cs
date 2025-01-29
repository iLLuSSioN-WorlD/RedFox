using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class GamesCommand : ICommand
    {
        public string CommandName => "games";

        public async Task ExecuteAsync(IMessageChannel channel, IUser user, string[] args)
        {
            var builder = new ComponentBuilder()
                .WithButton("🎲 Roll", "roll_cmd", ButtonStyle.Primary)
                .WithButton("🪙 Coin", "coin_cmd", ButtonStyle.Secondary)
                .WithButton("🎲 Dice", "dice_cmd", ButtonStyle.Success)
                .WithButton("✊✋✌ RPS", "rps_cmd", ButtonStyle.Danger);

            var message = $"<@{user.Id}>, выберите игру:";
            await channel.SendMessageAsync(message, components: builder.Build());
        }

        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            var builder = new ComponentBuilder()
                .WithButton("🎲 Roll", "roll_cmd", ButtonStyle.Primary)
                .WithButton("🪙 Coin", "coin_cmd", ButtonStyle.Secondary)
                .WithButton("🎲 Dice", "dice_cmd", ButtonStyle.Success)
                .WithButton("✊✋✌ RPS", "rps_cmd", ButtonStyle.Danger);

            await command.RespondAsync("Выберите игру:", components: builder.Build());
        }

        public async Task ExecuteComponentCommandAsync(SocketMessageComponent component) // ✅ Добавили этот метод
        {
            var builder = new ComponentBuilder()
                .WithButton("🎲 Roll", "roll_cmd", ButtonStyle.Primary)
                .WithButton("🪙 Coin", "coin_cmd", ButtonStyle.Secondary)
                .WithButton("🎲 Dice", "dice_cmd", ButtonStyle.Success)
                .WithButton("✊✋✌ RPS", "rps_cmd", ButtonStyle.Danger);

            await component.RespondAsync("Выберите игру:", components: builder.Build());
        }

        public ApplicationCommandProperties RegisterSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("games")
                .WithDescription("Показать список доступных игр")
                .Build();
        }
    }
}
