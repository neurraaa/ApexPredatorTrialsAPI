using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Services
{
    public class AdminBootstrapService
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminBootstrapService> _logger;

        public AdminBootstrapService(
            IUserRepository repository,
            IConfiguration configuration,
            ILogger<AdminBootstrapService> logger)
        {
            _repository = repository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task EnsureAdminAsync(CancellationToken cancellationToken = default)
        {
            var username = _configuration["AdminBootstrap:Username"]?.Trim();
            var password = _configuration["AdminBootstrap:Password"];

            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Admin bootstrap is not configured. No initial admin account will be created.");
                return;
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Admin bootstrap requires both AdminBootstrap:Username and AdminBootstrap:Password.");

            if (password.Length < 8)
                throw new InvalidOperationException("Admin bootstrap password must contain at least 8 characters.");

            var existingUser = await _repository.GetByUsernameAsync(username);

            if (existingUser is not null)
            {
                var changed = false;
                if (!existingUser.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    existingUser.Role = "Admin";
                    changed = true;
                }

                if (!BCrypt.Net.BCrypt.Verify(password, existingUser.PasswordHash))
                {
                    existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                    changed = true;
                }

                if (changed)
                {
                    _repository.Update(existingUser);
                    await _repository.SaveChangesAsync();
                    _logger.LogInformation("Synchronized the configured bootstrap admin account {Username}", username);
                }

                return;
            }

            var admin = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Admin"
            };

            await _repository.AddAsync(admin);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Created the configured bootstrap admin account {Username}", username);
        }
    }
}
