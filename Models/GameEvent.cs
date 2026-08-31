using ApexPredatorTrialsAPI.Interfaces;

namespace ApexPredatorTrialsAPI.Models
{
    public class GameEvent : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Region { get; set; } = string.Empty;
        public int OrganizerId { get; set; }

        public User Organizer { get; set; } = null!;
        public ICollection<GameEventRegistration> EventRegistrations { get; set; } = new List<GameEventRegistration>();
        public ICollection<GameEventSchedule> Schedules { get; set; } = new List<GameEventSchedule>();
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
