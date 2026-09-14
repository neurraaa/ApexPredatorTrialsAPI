using System.ComponentModel.DataAnnotations;

namespace ApexPredatorTrialsAPI.DTOs
{
    public class ScheduleMatchResultDto
    {
        public int WinnerId { get; set; }
        public int LoserId { get; set; }
        [Range(0, int.MaxValue)] public int WinnerDeaths { get; set; }
        [Range(0, int.MaxValue)] public int WinnerKills { get; set; }
        [Range(0, int.MaxValue)] public int LoserDeaths { get; set; }
        [Range(0, int.MaxValue)] public int LoserKills { get; set; }
        [Range(0, 5)] public int NestsDestroyed { get; set; }
    }
}
