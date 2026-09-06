using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Services;

namespace ApexPredatorTrialsAPI.Interfaces
{
    public interface IMatchService
    {
        Task<List<MatchDto>> GetAllAsync();
        Task<MatchDto?> GetByIdAsync(int id);
        Task<ServiceResult<MatchDto>> CreateAsync(MatchWriteDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, MatchWriteDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
