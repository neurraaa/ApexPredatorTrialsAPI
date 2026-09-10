using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ApexPredatorTrialsAPI.Services
{
    public class AdminBootstrapService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminBootstrapService> _logger;

        public AdminBootstrapService(
            AppDbContext context,
            IConfiguration configuration,
            ILogger<AdminBootstrapService> logger)
        {
            _context = context;
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
            {
                throw new InvalidOperationException(
                    "Admin bootstrap requires both AdminBootstrap__Username and AdminBootstrap__Password.");
            }

            if (password.Length < 8)
            {
                throw new InvalidOperationException("Admin bootstrap password must contain at least 8 characters.");
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(user => user.Username == username, cancellationToken);

            if (existingUser != null)
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
                    await _context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Synchronized the configured bootstrap admin account {Username}.", username);
                }

                return;
            }

            _context.Users.Add(new User
            {
                Username = string.IsNullOrWhiteSpace(username) ? "Administrator" : username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Admin",
            });

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Created the configured bootstrap admin account {Username}.", username);
        }
    }
}
