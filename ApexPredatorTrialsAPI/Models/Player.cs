using ApexPredatorTrialsAPI.Interfaces;

namespace ApexPredatorTrialsAPI.Models
{
    public class Player : IEntity
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string Region {  get; set; } = string.Empty;
        public int PlayerStatsId { get; set; }

        public User? User { get; set; }
        public PlayerStats PlayerStats { get; set; } = null!;
        public ICollection<GameEventRegistration> EventRegistrations { get; set; } = new List<GameEventRegistration>();
        public ICollection<Match> HunterMatches { get; set; } = new List<Match>();
        public ICollection<Match> HumanMatches { get; set; } = new List<Match>();
    }
}
