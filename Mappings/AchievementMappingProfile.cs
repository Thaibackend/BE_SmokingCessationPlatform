using AutoMapper;
using SmokingQuitSupportAPI.Models.DTOs.Achievement;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Mappings
{
    public class AchievementMappingProfile : Profile
    {
        public AchievementMappingProfile()
        {
            CreateMap<Achievement, AchievementDto>();
            CreateMap<CreateAchievementRequestDto, Achievement>();

            CreateMap<UserAchievement, UserAchievementDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.AchievementName, opt => opt.MapFrom(src => src.Achievement.Name))
                .ForMember(dest => dest.AchievementIcon, opt => opt.MapFrom(src => src.Achievement.Icon));
        }
    }
} 