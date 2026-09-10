using System.ComponentModel.DataAnnotations;

namespace ApexPredatorTrialsAPI.DTOs
{
    public class PlayerStatsDto
    {
        public int Id { get; set; }
        public string HunterRank { get; set; } = string.Empty;
        public int HunterHoursPlayed { get; set; }
        public int MutationLevel { get; set; }
        public string HumanRank { get; set; } = string.Empty;
        public int HumanHoursPlayed { get; set; }
        public int LegendLevel { get; set; }
        public int HoursTotal { get; set; }
    }
    public class PlayerStatsWriteDto
    {
        [Required] public string HunterRank { get; set; } = string.Empty;
        [Range(0, int.MaxValue)] public int HunterHoursPlayed { get; set; }
        [Range(1, 3)] public int MutationLevel { get; set; }
        [Required] public string HumanRank { get; set; } = string.Empty;
        [Range(0, int.MaxValue)] public int HumanHoursPlayed { get; set; }
        [Range(1, 250)] public int LegendLevel { get; set; }
    }
}
