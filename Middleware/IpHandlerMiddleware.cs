using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;

namespace ApexPredatorTrialsAPI.Middleware
{
    public class IpHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IpHandlerMiddleware> _logger;
        public IpHandlerMiddleware(RequestDelegate next, ILogger<IpHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IIpAddress ipAddresses)
        {
            string clientAddress = context.Connection.RemoteIpAddress?.ToString() ?? throw new Exception("Address not found");

            try
            {
                IpAddress? ipAddress = ipAddresses.Get(clientAddress);

                if (ipAddress is null)
                {
                    _logger.LogInformation("New IP address {ClientAddress} logged", clientAddress);
                    ipAddresses.Add(new() { Address = clientAddress, IsBlocked = false });
                    await _next(context);
                    return;
                }

                if (ipAddress.IsBlocked)
                {
                    _logger.LogWarning("Blocked IP {ClientAddress} attempted access", clientAddress);

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
                else
                {
                    await _next(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IpHandlerMiddleware: unexpected error for {ClientAddress}", clientAddress);

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }
            }
        }
    }
}
