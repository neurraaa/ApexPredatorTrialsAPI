using System.ComponentModel.DataAnnotations;

namespace ApexPredatorTrialsAPI.DTOs
{
    public class GameEventCreateDto
    {
        [Required] public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required] public string Region { get; set; } = string.Empty;
        [Required] public string StartingRound { get; set; } = string.Empty;
        [Required, MinLength(1)] public List<MatchupCreateDto> Matchups { get; set; } = new();
    }

    public class MatchupCreateDto
    {
        [Required] public EventPlayerEntryDto Hunter { get; set; } = null!;
        [Required] public EventPlayerEntryDto Human { get; set; } = null!;
    }

    public class EventPlayerEntryDto
    {
        public int? PlayerId { get; set; }
        public string? Name { get; set; }
        public string? Platform { get; set; }
        public string? Region { get; set; }
        [Required] public PlayerStatsWriteDto Stats { get; set; } = null!;
    }
}
