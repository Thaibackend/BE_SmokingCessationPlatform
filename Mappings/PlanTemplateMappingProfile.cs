using AutoMapper;
using SmokingQuitSupportAPI.Models.DTOs.PlanTemplate;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Mappings
{
    public class PlanTemplateMappingProfile : Profile
    {
        public PlanTemplateMappingProfile()
        {
            CreateMap<PlanTemplate, PlanTemplateDto>();
            CreateMap<CreatePlanTemplateRequestDto, PlanTemplate>();
        }
    }
} 