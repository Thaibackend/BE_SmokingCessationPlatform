using AutoMapper;
using SmokingQuitSupportAPI.Models.DTOs.CoachApplication;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Mappings
{
    public class CoachApplicationMappingProfile : Profile
    {
        public CoachApplicationMappingProfile()
        {
            CreateMap<CoachApplication, CoachApplicationDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
            CreateMap<CreateCoachApplicationRequestDto, CoachApplication>();
        }
    }
} 