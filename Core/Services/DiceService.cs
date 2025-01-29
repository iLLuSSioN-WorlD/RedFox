namespace DiscordBot.Services
{
    public class DiceService
    {
        private readonly DiceRoller _diceRoller = new();

        public int Roll(int sides)
        {
            return _diceRoller.Roll(sides);
        }
    }
}
