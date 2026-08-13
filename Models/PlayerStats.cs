using ApexPredatorTrialsAPI.Interfaces;

namespace ApexPredatorTrialsAPI.Models
{
    public class PlayerStats : IEntity
    {
        public int Id { get; set; }
        public string HunterRank { get; set; } = string.Empty;
        public int HunterHoursPlayed { get; set; }
        public int MutationLevel { get; set; }
        public string HumanRank { get; set; } = string.Empty;
        public int HumanHoursPlayed { get; set; }
        public int LegendLevel {  get; set; }

        public int HoursTotal => HunterHoursPlayed + HumanHoursPlayed;
    }
}
