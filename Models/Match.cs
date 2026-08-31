using ApexPredatorTrialsAPI.Interfaces;

namespace ApexPredatorTrialsAPI.Models
{
    public class Match : IEntity
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Region { get; set; } = string.Empty;
        public int HunterPlayerId { get; set; }
        public int HumanPlayerId { get; set; }
        public int? EventId { get; set; }
        public int MapId { get; set; }

        public GameEvent? Event { get; set; }
        public Player HunterPlayer { get; set; } = null!;
        public Player HumanPlayer { get; set; } = null!;
        public GameMap Map { get; set; } = null!;
        public MatchResults? MatchResults { get; set; }
        public GameEventSchedule? Schedule { get; set; }
    }
}
