using ApexPredatorTrialsAPI.Interfaces;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IRepository<Player>, InMemoryRepository<Player>>();
builder.Services.AddSingleton<IRepository<PlayerStats>, InMemoryRepository<PlayerStats>>();
builder.Services.AddSingleton<IRepository<GameMap>, InMemoryRepository<GameMap>>();
builder.Services.AddSingleton<IRepository<Match>, InMemoryRepository<Match>>();
builder.Services.AddSingleton<IRepository<MatchResults>, InMemoryRepository<MatchResults>>();
builder.Services.AddSingleton<IRepository<GameEvent>, InMemoryRepository<GameEvent>>();
builder.Services.AddSingleton<IRepository<GameEventRegistration>, InMemoryRepository<GameEventRegistration>>();
builder.Services.AddSingleton<IRepository<GameEventSchedule>, InMemoryRepository<GameEventSchedule>>();
builder.Services.AddSingleton<IRepository<User>, InMemoryRepository<User>>();

var app = builder.Build();

app.MapControllers();

app.Run();
