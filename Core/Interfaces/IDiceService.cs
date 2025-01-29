namespace DiscordBot.Services
{
    public interface IDiceService
    {
        int Roll(int sides);
        int[] RollMultiple(int sides, int count);  // Новый метод для нескольких бросков
        bool IsValidSides(int sides);
        string GetValidSidesList();
    }
}
