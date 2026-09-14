using System.ComponentModel.DataAnnotations;

namespace ApexPredatorTrialsAPI.DTOs
{
    public class AdvancePhaseDto
    {
        [Required] public string NextRound { get; set; } = string.Empty;
        [Required, MinLength(1)] public List<ScheduleWinnerDto> Winners { get; set; } = new();
        [Required, MinLength(1)] public List<NextMatchupDto> NextMatchups { get; set; } = new();
    }

    public class ScheduleWinnerDto
    {
        public int ScheduleId { get; set; }
        public int WinnerPlayerId { get; set; }
    }

    public class NextMatchupDto
    {
        public int HunterScheduleId { get; set; }
        public int HumanScheduleId { get; set; }
    }
}
