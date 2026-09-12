using ApexPredatorTrialsAPI.Common;
using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Middleware;
using ApexPredatorTrialsAPI.Repositories;
using ApexPredatorTrialsAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ApexPredatorTrialsAPI", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste admin token here."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

// repos
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerStatsRepository, PlayerStatsRepository>();
builder.Services.AddScoped<IGameMapRepository, GameMapRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchResultsRepository, MatchResultsRepository>();
builder.Services.AddScoped<IGameEventRepository, GameEventRepository>();
builder.Services.AddScoped<IGameEventRegistrationRepository, GameEventRegistrationRepository>();
builder.Services.AddScoped<IGameEventScheduleRepository, GameEventScheduleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// services
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerStatsService, PlayerStatsService>();
builder.Services.AddScoped<IGameMapService, GameMapService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IMatchResultsService, MatchResultsService>();
builder.Services.AddScoped<IGameEventService, GameEventService>();
builder.Services.AddScoped<IGameEventRegistrationService, GameEventRegistrationService>();
builder.Services.AddScoped<IGameEventScheduleService, GameEventScheduleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IIpAddress, IpAddressService>();

// auth
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AdminBootstrapService>();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<IpHandlerMiddleware>();
app.UseMiddleware<RateLimiterMiddleware>();
app.UseMiddleware<TimingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var bootstrapService = scope.ServiceProvider.GetRequiredService<AdminBootstrapService>();
    await bootstrapService.EnsureAdminAsync();
}

app.Run();