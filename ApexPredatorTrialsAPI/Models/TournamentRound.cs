using System.Linq;

namespace ApexPredatorTrialsAPI.Models
{
    public static class TournamentRound
    {
        public static readonly string[] Order = { "Quarter-Final", "Semi-Final", "Final" };

        public static bool IsValid(string round) => Order.Contains(round);

        public static string? NextAfter(string round)
        {
            var index = Array.IndexOf(Order, round);
            return index >= 0 && index < Order.Length - 1 ? Order[index + 1] : null;
        }
    }
}
