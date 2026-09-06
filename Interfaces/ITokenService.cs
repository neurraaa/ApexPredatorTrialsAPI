using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
