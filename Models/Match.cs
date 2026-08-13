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
        public int EventId { get; set; }
        public int MapId { get; set; }
    }
}
