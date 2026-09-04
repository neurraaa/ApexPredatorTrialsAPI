using AutoMapper;
using ApexPredatorTrialsAPI.Models;
using ApexPredatorTrialsAPI.DTOs;

namespace ApexPredatorTrialsAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Player, PlayerDto>();
            CreateMap<PlayerWriteDto, Player>();

            // No ReverseMap on User - PasswordHash must never come from a DTO field copy.
            CreateMap<User, UserDto>();
            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // hashed explicitly in the controller

            CreateMap<PlayerStats, PlayerStatsDto>();
            CreateMap<PlayerStatsWriteDto, PlayerStats>();

            CreateMap<GameMap, GameMapDto>();
            CreateMap<GameMapWriteDto, GameMap>();

            CreateMap<GameEvent, GameEventDto>();
            CreateMap<GameEventWriteDto, GameEvent>();

            CreateMap<EventRegistration, EventRegistrationDto>();
            CreateMap<EventRegistrationCreateDto, EventRegistration>();

            CreateMap<Schedule, ScheduleDto>();
            CreateMap<ScheduleWriteDto, Schedule>();

            // Match -> MatchDto flattens HunterPlayer.Name -> HunterPlayerName etc.
            // automatically, since the controller Include()s those navigations.
            CreateMap<Match, MatchDto>();
            CreateMap<MatchWriteDto, Match>();

            CreateMap<MatchResults, MatchResultsDto>();
            CreateMap<MatchResultsCreateDto, MatchResults>();
        }
    }
}
