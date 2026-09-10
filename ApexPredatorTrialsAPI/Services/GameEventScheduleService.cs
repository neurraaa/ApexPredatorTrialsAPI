using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Models;
using AutoMapper;

namespace ApexPredatorTrialsAPI.Services
{
    public class GameEventScheduleService : IGameEventScheduleService
    {
        private readonly IGameEventScheduleRepository _repository;
        private readonly IMapper _mapper;

        public GameEventScheduleService(IGameEventScheduleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<GameEventScheduleDto>> GetAllAsync() =>
            _mapper.Map<List<GameEventScheduleDto>>(await _repository.GetAllAsync());

        public async Task<GameEventScheduleDto?> GetByIdAsync(int id)
        {
            var schedule = await _repository.GetByIdWithMatchAsync(id);
            return schedule is null ? null : _mapper.Map<GameEventScheduleDto>(schedule);
        }

        public async Task<GameEventScheduleDto> CreateAsync(GameEventScheduleWriteDto dto)
        {
            var schedule = _mapper.Map<GameEventSchedule>(dto);
            await _repository.AddAsync(schedule);
            await _repository.SaveChangesAsync();
            return _mapper.Map<GameEventScheduleDto>(schedule);
        }

        public async Task<bool> UpdateAsync(int id, GameEventScheduleWriteDto dto)
        {
            var schedule = await _repository.GetByIdAsync(id);
            if (schedule is null) return false;

            _mapper.Map(dto, schedule);
            _repository.Update(schedule);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var schedule = await _repository.GetByIdAsync(id);
            if (schedule is null) return false;

            _repository.Delete(schedule);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
