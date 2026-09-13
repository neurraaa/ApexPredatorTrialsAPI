using ApexPredatorTrialsAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ApexPredatorTrialsAPI.Middleware
{
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimiterMiddleware> _logger;

        public RateLimiterMiddleware(RequestDelegate next, ILogger<RateLimiterMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext, AppDbContext dbContext)
        {
            var clientAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var windowStart = DateTime.UtcNow.AddMinutes(-1);
            
            var requestCount = await dbContext
                .Logs
                .Where(l => l.ClientAddress == clientAddress && l.Timestamp > windowStart)
                .CountAsync();

            if (requestCount >= 100)
            {
                _logger.LogWarning("Rate limit exceeded for {ClientAddress} ({RequestCount} requests)", clientAddress, requestCount);

                httpContext.Response.Headers.Append("Access-Control-Allow-Origin", "http://127.0.0.1:5500");
                httpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                httpContext.Response.ContentType = "text/plain";

                await httpContext.Response.WriteAsync("Rate limit exceeded");

                return;
            }

            await _next(httpContext);
        }
    }
}
