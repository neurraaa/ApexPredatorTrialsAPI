using System.Linq;

namespace ApexPredatorTrialsAPI.Models
{
    public static class TournamentRound
    {
        public static readonly string[] Order = { "Quarter-Final", "Semi-Final", "Final" };

        private static readonly Dictionary<string, int> RequiredStartingMatchups = new()
        {
            ["Quarter-Final"] = 4,
            ["Semi-Final"] = 2,
            ["Final"] = 1
        };

        public static bool IsValid(string round) => Order.Contains(round);

        public static string? NextAfter(string round)
        {
            var index = Array.IndexOf(Order, round);
            return index >= 0 && index < Order.Length - 1 ? Order[index + 1] : null;
        }

        public static int? RequiredStartingMatchupCount(string round) =>
            RequiredStartingMatchups.TryGetValue(round, out var count) ? count : null;
    }
}
