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

        // Обработка команды в сообщениях (не слэш)
        public async Task ExecuteAsync(IMessageChannel channel, IUser user, string[] args)
        {
            var coinFlipResult = _randomService.Generate(0, 1) == 0 ? "Орел" : "Решка";
            var message = $"<@{user.Id}> подбрасывает монету и выпадает: {coinFlipResult}";
            await channel.SendMessageAsync(message);
        }

        // Обработка слэш-команды
        public async Task ExecuteSlashCommandAsync(SocketSlashCommand command)
        {
            var coinFlipResult = _randomService.Generate(0, 1) == 0 ? "Орел" : "Решка";
            await command.RespondAsync($"Результат подбрасывания монеты: {coinFlipResult}");
        }

        // Обработка кнопок
        public async Task ExecuteComponentCommandAsync(SocketMessageComponent component)
        {
            // Генерируем результат подбрасывания монеты
            var coinFlipResult = _randomService.Generate(0, 1) == 0 ? "Орел" : "Решка";

            // Отправляем результат в ответ на клик по кнопке
            await component.RespondAsync($"🪙 Бросаю монету... Выпало: {coinFlipResult}");
        }

        // Регистрация слэш-команды
        public ApplicationCommandProperties RegisterSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("coin")
                .WithDescription("Подбросить монету (Орел или Решка)")
                .Build();
        }
    }
}
