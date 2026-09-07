using ApexPredatorTrialsAPI.Data;
using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Repositories;
using ApexPredatorTrialsAPI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();