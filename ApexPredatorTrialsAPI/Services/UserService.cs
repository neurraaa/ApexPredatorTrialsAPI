using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Models;
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

        public async Task<ServiceResult<UserDto>> RegisterAsync(UserRegisterDto dto)
        {
            if (await _repository.UsernameExistsAsync(dto.Username))
                return ServiceResult<UserDto>.Invalid("Username is already taken.");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Role = "user";

            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();

            return ServiceResult<UserDto>.Ok(_mapper.Map<UserDto>(user));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user is null) return false;

            _repository.Delete(user);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
