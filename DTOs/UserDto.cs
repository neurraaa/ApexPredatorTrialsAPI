using System.ComponentModel.DataAnnotations;

namespace ApexPredatorTrialsAPI.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
    public class UserRegisterDto
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required, MinLength(8)] public string Password { get; set; } = string.Empty;
    }
}
