using ApexPredatorTrialsAPI.Interfaces;

namespace ApexPredatorTrialsAPI.Models
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "user";

        public Player? Player { get; set; }
        public ICollection<GameEvent> OrganizedEvents { get; set; } = new List<GameEvent>();
    }
}
