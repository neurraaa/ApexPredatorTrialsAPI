using System.ComponentModel.DataAnnotations;

namespace ApexPredatorTrialsAPI.DTOs
{
    public class GameMapDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class GameMapWriteDto
    {
        [Required] public string Name { get; set; } = string.Empty;
    }
}
