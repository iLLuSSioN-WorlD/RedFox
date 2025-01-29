using System;
using System.Linq;

namespace DiscordBot.Services
{
    public class DiceService : IDiceService
    {
        private readonly Random _random = new Random();

        // Бросок одного кубика
        public int Roll(int sides)
        {
            return _random.Next(1, sides + 1);
        }

        // Бросок нескольких кубиков
        public int[] RollMultiple(int sides, int numDice)
        {
            return Enumerable.Range(0, numDice).Select(_ => Roll(sides)).ToArray();
        }

        // Проверка валидности количества граней
        public bool IsValidSides(int sides)
        {
            return sides > 1 && sides <= 100;
        }

        // Получение списка допустимых граней
        public string GetValidSidesList()
        {
            return "2, 4, 6, 8, 10, 12, 20"; // Например, ограничиваем допустимыми значениями
        }
    }
}
