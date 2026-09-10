using ApexPredatorTrialsAPI.Interfaces;

namespace ApexPredatorTrialsAPI.Models
{
    public class GameEventRegistration : IEntity
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int PlayerId { get; set; }
        public DateTime DateRegistered { get; set; }

        public GameEvent Event { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
