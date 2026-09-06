using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.DTOs;
using AutoMapper;

namespace ApexPredatorTrialsAPI.Services
{
    public class GameEventRegistrationService : IGameEventRegistrationService
    {
        private readonly IGameEventRegistrationRepository _repository;
        private readonly IMapper _mapper;

        public GameEventRegistrationService(IGameEventRegistrationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<GameEventRegistrationDto>> GetAllAsync() =>
            _mapper.Map<List<GameEventRegistrationDto>>(await _repository.GetAllAsync());

        public async Task<GameEventRegistrationDto?> GetByIdAsync(int id)
        {
            var reg = await _repository.GetByIdAsync(id);
            return reg is null ? null : _mapper.Map<GameEventRegistrationDto>(reg);
        }

        public async Task<ServiceResult<GameEventRegistrationDto>> CreateAsync(GameEventRegistrationCreateDto dto)
        {
            if (await _repository.IsPlayerRegisteredAsync(dto.EventId, dto.PlayerId))
                return ServiceResult<GameEventRegistrationDto>.Invalid("This player is already registered for this event.");

            var reg = _mapper.Map<GameEventRegistration>(dto);
            reg.DateRegistered = DateTime.UtcNow;

            await _repository.AddAsync(reg);
            await _repository.SaveChangesAsync();

            return ServiceResult<GameEventRegistrationDto>.Ok(_mapper.Map<GameEventRegistrationDto>(reg));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var reg = await _repository.GetByIdAsync(id);
            if (reg is null) return false;

            _repository.Delete(reg);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
