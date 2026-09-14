using System.ComponentModel.DataAnnotations;

namespace ApexPredatorTrialsAPI.DTOs
{
    public class GameEventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Region { get; set; } = string.Empty;
        public int OrganizerId { get; set; }
        public string StartingRound { get; set; } = string.Empty;
        public string CurrentRound { get; set; } = string.Empty;
    }
    public class GameEventWriteDto
    {
        [Required] public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required] public string Region { get; set; } = string.Empty;
    }
}
