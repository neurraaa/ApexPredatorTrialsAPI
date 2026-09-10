using ApexPredatorTrialsAPI.Interfaces;

namespace ApexPredatorTrialsAPI.Models
{
    public class MatchResults : IEntity
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int WinnerId { get; set; }
        public int LoserId { get; set; }
        public int WinnerDeaths { get; set; }
        public int LoserDeaths { get; set; }
        public int WinnerKills { get; set; }
        public int LoserKills { get; set; }
        public int NestsDestroyed { get; set; }
        public DateTime DateTimeConcluded { get; set; }

        public Match Match { get; set; } = null!;
        public Player Winner { get; set; } = null!;
        public Player Loser { get; set; } = null!;
    }
}
