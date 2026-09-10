using System.ComponentModel.DataAnnotations;

namespace ApexPredatorTrialsAPI.DTOs
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int PlayerStatsId { get; set; }
    }
    public class PlayerWriteDto
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Platform { get; set; } = string.Empty;
        [Required] public string Region { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int PlayerStatsId { get; set; }
    }
}
