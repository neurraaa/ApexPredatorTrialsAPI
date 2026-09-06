using ApexPredatorTrialsAPI.DTOs;
using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public AuthService(
            AppDbContext context,
            ITokenService tokenService,
            IMapper mapper,
            IWebHostEnvironment environment)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
            _environment = environment;
        }

        public async Task<AuthResponseDto> RegisterAsync(UserRegisterDto dto)
        {
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var requestedRole = dto.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                ? "Admin"
                : "User";
            if (requestedRole == "Admin" && !_environment.IsDevelopment())
            {
                throw new InvalidOperationException("Admin self-registration is available only in Development.");
            }
            user.Role = requestedRole;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new InvalidOperationException("Invalid username or password.");
            }

            var token = _tokenService.GenerateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.OrderBy(user => user.Id).ToListAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            if (user.Role == "Admin")
            {
                var adminCount = await _context.Users.CountAsync(candidate => candidate.Role == "Admin");
                if (adminCount <= 1)
                {
                    throw new InvalidOperationException("The last administrator cannot be deleted.");
                }
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
