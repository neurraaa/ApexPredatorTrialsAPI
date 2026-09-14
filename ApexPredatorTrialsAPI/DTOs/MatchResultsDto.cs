using System.ComponentModel.DataAnnotations;

namespace ApexPredatorTrialsAPI.DTOs
{
    public class MatchResultsDto
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int WinnerId { get; set; }
        public string WinnerName { get; set; } = string.Empty;
        public int LoserId { get; set; }
        public string LoserName { get; set; } = string.Empty;
        public int WinnerDeaths { get; set; }
        public int WinnerKills { get; set; }
        public int LoserDeaths { get; set; }
        public int LoserKills { get; set; }
        public int NestsDestroyed { get; set; }
    }
    public class MatchResultsCreateDto
    {
        public int MatchId { get; set; }
        public int WinnerId { get; set; }
        public int LoserId { get; set; }
        [Range(0, int.MaxValue)] public int WinnerDeaths { get; set; }
        [Range(0, int.MaxValue)] public int WinnerKills { get; set; }
        [Range(0, int.MaxValue)] public int LoserDeaths { get; set; }
        [Range(0, int.MaxValue)] public int LoserKills { get; set; }
        [Range(0, 5)] public int NestsDestroyed { get; set; }
    }
}
