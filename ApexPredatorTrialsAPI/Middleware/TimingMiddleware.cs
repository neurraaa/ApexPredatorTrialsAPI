using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Models;
using System.Diagnostics;

namespace ApexPredatorTrialsAPI.Middleware
{
    public class TimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TimingMiddleware> _logger;

        public TimingMiddleware(RequestDelegate next, ILogger<TimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext, AppDbContext dbContext)
        {
            var sw = Stopwatch.StartNew();

            await _next(httpContext);

            sw.Stop();

            var duration = sw.Elapsed.TotalMilliseconds;

            var clientAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var method = httpContext.Request.Method;
            var path = httpContext.Request.Path;

            _logger.LogInformation("{Method} {Path} from {ClientAddress} took {Duration} ms", method, path, clientAddress, duration);

            Log log = new()
            {
                ClientAddress = clientAddress,
                Method = method,
                Path = path,
                Duration = duration,
                Timestamp = DateTime.UtcNow
            };

            dbContext.Logs.Add(log);

            await dbContext.SaveChangesAsync();
        }
    }
}
