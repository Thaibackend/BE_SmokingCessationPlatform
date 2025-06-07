using AutoMapper;
using SmokingQuitSupportAPI.Models.DTOs.SmokingStatus;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Mappings
{
    public class SmokingStatusMappingProfile : Profile
    {
        public SmokingStatusMappingProfile()
        {
            CreateMap<SmokingStatus, SmokingStatusDto>();
            CreateMap<CreateSmokingStatusRequestDto, SmokingStatus>();
        }
    }
} 