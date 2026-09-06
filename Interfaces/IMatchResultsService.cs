using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Services;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IMatchResultsService
    {
        Task<List<MatchResultsDto>> GetAllAsync();
        Task<MatchResultsDto?> GetByIdAsync(int id);
        Task<ServiceResult<MatchResultsDto>> CreateAsync(MatchResultsCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
