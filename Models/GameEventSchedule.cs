using ApexPredatorTrialsAPI.Interfaces;

namespace ApexPredatorTrialsAPI.Models
{
    public class GameEventSchedule : IEntity
    {
        public int Id { get; set; }
        public int EventId { get; set; } 
        public string Round { get; set; } = string.Empty;
        public int? MatchId { get; set; }
        public int? NextScheduleId { get; set; }

        public GameEvent Event { get; set; } = null!;
        public Match? Match { get; set; }
        public GameEventSchedule? NextSchedule { get; set; }
        public ICollection<GameEventSchedule> PreviousSchedules { get; set; } = new List<GameEventSchedule>();
    }
}
