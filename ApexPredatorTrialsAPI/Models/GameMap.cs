using ApexPredatorTrialsAPI.Interfaces;

namespace ApexPredatorTrialsAPI.Models
{
    public class GameMap : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
