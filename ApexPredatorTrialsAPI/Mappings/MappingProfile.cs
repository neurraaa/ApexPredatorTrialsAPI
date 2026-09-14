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
            CreateMap<Player, PlayerDetailDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null))
                .ForMember(dest => dest.Stats, opt => opt.MapFrom(src => src.PlayerStats));

            CreateMap<User, UserDto>();
            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<PlayerStats, PlayerStatsDto>();
            CreateMap<PlayerStatsWriteDto, PlayerStats>();

            CreateMap<GameMap, GameMapDto>();
            CreateMap<GameMapWriteDto, GameMap>();

            CreateMap<GameEvent, GameEventDto>();
            CreateMap<GameEventWriteDto, GameEvent>();

            CreateMap<GameEventRegistration, GameEventRegistrationDto>();
            CreateMap<GameEventRegistrationCreateDto, GameEventRegistration>();

            CreateMap<Match, MatchDto>();
            CreateMap<MatchWriteDto, Match>();

            CreateMap<GameEventSchedule, GameEventScheduleDto>()
                .ForMember(dest => dest.HunterPlayerName, opt => opt.MapFrom(src => src.HunterPlayer != null ? src.HunterPlayer.Name : null))
                .ForMember(dest => dest.HumanPlayerName, opt => opt.MapFrom(src => src.HumanPlayer != null ? src.HumanPlayer.Name : null));
            CreateMap<GameEventScheduleWriteDto, GameEventSchedule>();

            CreateMap<MatchResults, MatchResultsDto>();
            CreateMap<MatchResultsCreateDto, MatchResults>();
        }
    }
}
