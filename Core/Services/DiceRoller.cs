using System;

namespace DiscordBot
{
    public class DiceRoller
    {
        private readonly Random _random = new();

        // Бросок кубика с заданным количеством граней
        public int Roll(int sides)
        {
            return _random.Next(1, sides + 1);
        }
    }
}
