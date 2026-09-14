using System.ComponentModel.DataAnnotations;

namespace ApexPredatorTrialsAPI.DTOs
{
    public class GameEventScheduleDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Round { get; set; } = string.Empty;
        public int? MatchId { get; set; }
        public int? NextScheduleId { get; set; }
        public int? HunterPlayerId { get; set; }
        public string? HunterPlayerName { get; set; }
        public int? HumanPlayerId { get; set; }
        public string? HumanPlayerName { get; set; }
        public int? WinnerPlayerId { get; set; }
        public int? MapId { get; set; }
        public string? MapName { get; set; }
    }
    public class GameEventScheduleWriteDto
    {
        public int EventId { get; set; }
        [Required] public string Round { get; set; } = string.Empty;
        public int? MatchId { get; set; }
        public int? NextScheduleId { get; set; }
    }
}