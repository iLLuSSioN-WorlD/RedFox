using Discord.WebSocket;
using DiscordBot.Commands;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class ButtonHandler
    {
        private readonly CoinCommand _coinCommand;
        private readonly DiceCommand _diceCommand;
        private readonly RollCommand _rollCommand;
        private readonly RpsCommand _rpsCommand;

        public ButtonHandler(CoinCommand coinCommand, DiceCommand diceCommand, RollCommand rollCommand, RpsCommand rpsCommand)
        {
            _coinCommand = coinCommand;
            _diceCommand = diceCommand;
            _rollCommand = rollCommand;
            _rpsCommand = rpsCommand;
        }

        public async Task HandleButtonClick(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "coin_cmd":
                    await _coinCommand.ExecuteComponentCommandAsync(component);
                    break;
                case "dice_cmd":
                    await _diceCommand.ExecuteComponentCommandAsync(component);
                    break;
                case "roll_cmd":
                    await _rollCommand.ExecuteComponentCommandAsync(component);
                    break;
                case "rps_cmd":
                    await _rpsCommand.ExecuteComponentCommandAsync(component);
                    break;
                default:
                    await component.RespondAsync("❌ Неизвестная кнопка!", ephemeral: true);
                    break;
            }
        }
    }
}
