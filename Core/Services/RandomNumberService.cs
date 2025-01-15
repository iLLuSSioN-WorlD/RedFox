using System;

namespace DiscordBot
{
    public class RandomNumberService
    {
        private readonly Random _random = new();

        public int Generate(int min, int max)
        {
            return _random.Next(min, max + 1);
        }
    }
}