using AutoMapper;
using SmokingQuitSupportAPI.Models.DTOs.Notification;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Mappings
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
            CreateMap<Notification, NotificationDto>();
            CreateMap<CreateNotificationRequestDto, Notification>();
        }
    }
} 