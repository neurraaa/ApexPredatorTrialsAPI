namespace ApexPredatorTrialsAPI.DTOs
{
    public class MatchDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Region { get; set; } = string.Empty;
        public int? EventId { get; set; }
        public int HunterPlayerId { get; set; }
        public string HunterPlayerName { get; set; } = string.Empty;
        public int HumanPlayerId { get; set; }
        public string HumanPlayerName { get; set; } = string.Empty;
        public int MapId { get; set; }
        public string MapName { get; set; } = string.Empty;
    }
    public class MatchWriteDto
    {
        public DateTime DateTime { get; set; }
        public string Region { get; set; } = string.Empty;
        public int? EventId { get; set; }
        public int HunterPlayerId { get; set; }
        public int HumanPlayerId { get; set; }
        public int MapId { get; set; }
    }
}
