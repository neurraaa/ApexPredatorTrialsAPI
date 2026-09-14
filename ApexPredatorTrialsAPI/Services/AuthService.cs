using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.Interfaces;
using AutoMapper;

namespace ApexPredatorTrialsAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthService(
            IUserRepository repository,
            ITokenService tokenService,
            IMapper mapper)
        {
            _repository = repository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(UserRegisterDto dto)
        {
            if (await _repository.UsernameExistsAsync(dto.Username))
                return ServiceResult<AuthResponseDto>.Invalid("Username is already taken.");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Role = "User";

            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);
            return ServiceResult<AuthResponseDto>.Ok(new AuthResponseDto
            {
                Token = token,
                User = _mapper.Map<UserDto>(user)
            });
        }

        public async Task<ServiceResult<AuthResponseDto>> LoginAsync(UserLoginDto dto)
        {
            var user = await _repository.GetByUsernameAsync(dto.Username);
            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return ServiceResult<AuthResponseDto>.Invalid("Invalid username or password.");

            var token = _tokenService.GenerateToken(user);
            return ServiceResult<AuthResponseDto>.Ok(new AuthResponseDto
            {
                Token = token,
                User = _mapper.Map<UserDto>(user)
            });
        }
    }
}
