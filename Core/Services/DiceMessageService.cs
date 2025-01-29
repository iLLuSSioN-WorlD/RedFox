using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class DiceMessageService
    {
        // Генерация сообщения для броска нескольких кубиков
        public async Task<string> GenerateMultipleRollMessageAsync(string userName, int sides, int numDice, int[] results)
        {
            int sum = results.Sum();
            string rollResults = string.Join(", ", results); // Убираем кавычки вокруг чисел
            string diceNotation = numDice == 1 ? $"`d{sides}`" : $"`{numDice}d{sides}`";

            return numDice == 1
                ? $"🎲 **{userName}** бросает {diceNotation} и получает: `{results[0]}`."
                : $"🎲 **{userName}** бросает {diceNotation} и получает: `{rollResults}` (Сумма: **{sum}**)";
        }

        // Генерация сообщения для PvP броска
        public async Task<string> GeneratePvPMessageAsync(string userName, string opponentName, int sides, int numDice, int[] authorRolls, int[] opponentRolls, string outcome)
        {
            int authorSum = authorRolls.Sum();
            int opponentSum = opponentRolls.Sum();
            string authorResults = string.Join(", ", authorRolls); // Убираем кавычки вокруг чисел
            string opponentResults = string.Join(", ", opponentRolls); // Убираем кавычки вокруг чисел
            string diceNotation = numDice == 1 ? $"`d{sides}`" : $"`{numDice}d{sides}`";

            return numDice == 1
                ? $"🎲 **{userName}** вызвал **{opponentName}** на бросок `{diceNotation}`!\n\n" +
                  $"🎲 **{userName}** бросает и получает: `{authorResults}`\n" +
                  $"🎲 **{opponentName}** бросает и получает: `{opponentResults}`\n\n" +
                  $"🏆 **{userName}** {outcome}!"
                : $"🎲 **{userName}** вызвал **{opponentName}** на бросок `{diceNotation}`!\n\n" +
                  $"🎲 **{userName}** бросает и получает: `{authorResults}` (Сумма: **{authorSum}**)\n" +
                  $"🎲 **{opponentName}** бросает и получает: `{opponentResults}` (Сумма: **{opponentSum}**)\n\n" +
                  $"🏆 **{userName}** {outcome}!";
        }
    }
}
