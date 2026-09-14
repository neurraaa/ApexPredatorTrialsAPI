using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Services;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IMatchResultsService
    {
        Task<List<MatchResultsDto>> GetAllAsync();
        Task<MatchResultsDto?> GetByIdAsync(int id);
        Task<MatchResultsDto?> GetByMatchIdAsync(int matchId);
        Task<ServiceResult<MatchResultsDto>> CreateAsync(MatchResultsCreateDto dto);
        Task<ServiceResult<MatchResultsDto>> UpdateAsync(int id, MatchResultsCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
