using System.Collections.Generic;
using System.Threading.Tasks;

public interface IDiceMessageService
{
    Task<string> GenerateSingleRollMessageAsync(string userName, int sides, int result);
    Task<string> GenerateMultipleRollMessageAsync(string userName, int numDice, int sides, List<int> results);
    Task<string> GeneratePvPMessageAsync(string userName, string opponentName, int sides, int authorRoll, int opponentRoll, string outcome);
}
