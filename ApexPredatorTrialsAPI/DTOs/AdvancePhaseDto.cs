using System.ComponentModel.DataAnnotations;

namespace ApexPredatorTrialsAPI.DTOs
{
    public class AdvancePhaseDto
    {
        [Required] public string NextRound { get; set; } = string.Empty;
        [Required, MinLength(1)] public List<NextMatchupDto> NextMatchups { get; set; } = new();
    }

    public class NextMatchupDto
    {
        public int HunterScheduleId { get; set; }
        public int HumanScheduleId { get; set; }
        public int MapId { get; set; }
    }
}
