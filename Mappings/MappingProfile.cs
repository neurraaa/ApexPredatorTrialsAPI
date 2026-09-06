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

            CreateMap<User, UserDto>();
            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // hashed explicitly in controller

            CreateMap<PlayerStats, PlayerStatsDto>();
            CreateMap<PlayerStatsWriteDto, PlayerStats>();

            CreateMap<GameMap, GameMapDto>();
            CreateMap<GameMapWriteDto, GameMap>();

            CreateMap<GameEvent, GameEventDto>();
            CreateMap<GameEventWriteDto, GameEvent>();

            CreateMap<GameEventRegistration, GameEventRegistrationDto>();
            CreateMap<GameEventRegistrationCreateDto, GameEventRegistration>();

            CreateMap<GameEventSchedule, GameEventScheduleDto>();
            CreateMap<GameEventScheduleWriteDto, GameEventSchedule>();

            CreateMap<Match, MatchDto>();
            CreateMap<MatchWriteDto, Match>();

            CreateMap<MatchResults, MatchResultsDto>();
            CreateMap<MatchResultsCreateDto, MatchResults>();
        }
    }
}
