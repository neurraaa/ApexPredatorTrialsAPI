using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Interfaces;
using AutoMapper;

namespace ApexPredatorTrialsAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllAsync() =>
            _mapper.Map<List<UserDto>>(await _repository.GetAllAsync());

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            return user is null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user is null) return ServiceResult<bool>.NotFound();

            if (user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                var adminCount = await _repository.CountByRoleAsync("Admin");
                if (adminCount <= 1)
                    return ServiceResult<bool>.Invalid("The last administrator cannot be deleted.");
            }

            _repository.Delete(user);
            await _repository.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true);
        }
    }
}
