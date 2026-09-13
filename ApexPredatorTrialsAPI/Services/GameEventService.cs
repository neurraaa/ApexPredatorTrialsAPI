using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;
using AutoMapper;

namespace ApexPredatorTrialsAPI.Services
{
    public class GameEventService : IGameEventService
    {
        private readonly IGameEventRepository _repository;
        private readonly IGameEventScheduleRepository _scheduleRepository;
        private readonly IMapper _mapper;

        public GameEventService(IGameEventRepository repository, IGameEventScheduleRepository scheduleRepository, IMapper mapper)
        {
            _repository = repository;
            _scheduleRepository = scheduleRepository;
            _mapper = mapper;
        }

        public async Task<List<GameEventDto>> GetAllAsync() =>
            _mapper.Map<List<GameEventDto>>(await _repository.GetAllAsync());

        public async Task<GameEventDto?> GetByIdAsync(int id)
        {
            var ev = await _repository.GetByIdAsync(id);
            return ev is null ? null : _mapper.Map<GameEventDto>(ev);
        }

        public async Task<List<GameEventScheduleDto>> GetBracketAsync(int eventId) =>
            _mapper.Map<List<GameEventScheduleDto>>(await _scheduleRepository.GetByEventIdAsync(eventId));

        public async Task<GameEventDto> CreateAsync(GameEventWriteDto dto, int organizerId)
        {
            var ev = _mapper.Map<GameEvent>(dto);
            ev.OrganizerId = organizerId;
            await _repository.AddAsync(ev);
            await _repository.SaveChangesAsync();
            return _mapper.Map<GameEventDto>(ev);
        }

        public async Task<bool> UpdateAsync(int id, GameEventWriteDto dto)
        {
            var ev = await _repository.GetByIdAsync(id);
            if (ev is null) return false;

            _mapper.Map(dto, ev);
            _repository.Update(ev);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ev = await _repository.GetByIdAsync(id);
            if (ev is null) return false;

            _repository.Delete(ev);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
