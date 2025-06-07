using AutoMapper;
using SmokingQuitSupportAPI.Models.DTOs.DailyTask;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Mappings
{
    public class DailyTaskMappingProfile : Profile
    {
        public DailyTaskMappingProfile()
        {
            CreateMap<DailyTask, DailyTaskDto>();
            CreateMap<CreateDailyTaskRequestDto, DailyTask>();
        }
    }
} 