using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class CoinCommand : ICommand
    {
        private readonly RandomNumberService _randomService;

        public CoinCommand(RandomNumberService randomService)
        {
            _randomService = randomService;
        }

        public string CommandName => "coin";

        public async Task ExecuteAsync(IMessageChannel channel, IUser user, string[] args)
        {
            var coinFlipResult = _randomService.Generate(0, 1) == 0 ? "Орел" : "Решка";
            var message = $"<@{user.Id}> подбрасывает монету и выпадает: {coinFlipResult}";
            await channel.SendMessageAsync(message);
        }

        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            var coinFlipResult = _randomService.Generate(0, 1) == 0 ? "Орел" : "Решка";
            await command.RespondAsync($"Результат подбрасывания монеты: {coinFlipResult}");
        }

        public ApplicationCommandProperties RegisterSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("coin")
                .WithDescription("Подбросить монету (Орел или Решка)")
                .Build();
        }
    }
}
