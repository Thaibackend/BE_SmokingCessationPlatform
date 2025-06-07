using AutoMapper;
using SmokingQuitSupportAPI.Models.Entities;
using SmokingQuitSupportAPI.Models.DTOs.User;
using SmokingQuitSupportAPI.Models.DTOs.Activity;
using SmokingQuitSupportAPI.Models.DTOs.Post;
using SmokingQuitSupportAPI.Models.DTOs.Comment;
using SmokingQuitSupportAPI.Models.DTOs.Appointment;
using SmokingQuitSupportAPI.Models.DTOs.Package;
using SmokingQuitSupportAPI.Models.DTOs.Plan;
using SmokingQuitSupportAPI.Models.DTOs.Order;
using SmokingQuitSupportAPI.Models.DTOs.CoachCommission;
using CommentDto = SmokingQuitSupportAPI.Models.DTOs.Comment.CommentDto;

namespace SmokingQuitSupportAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<CreateUserRequestDto, User>();
            CreateMap<UpdateUserRequestDto, User>();

            // Activity mappings
            CreateMap<Activity, ActivityDto>();
            CreateMap<CreateActivityRequestDto, Activity>();

            // Post mappings
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User.Username));
            CreateMap<CreatePostRequestDto, Post>();
            CreateMap<UpdatePostRequestDto, Post>();

            // Comment mappings
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User.Username));

            // Appointment mappings
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Member.Username))
                .ForMember(dest => dest.CoachName, opt => opt.MapFrom(src => src.Coach.Username));
            CreateMap<CreateAppointmentRequestDto, Appointment>();

            // Package mappings
            CreateMap<Package, PackageDto>()
                .ForMember(dest => dest.CreatorId, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.Username : null))
                .ForMember(dest => dest.AssignedCoachName, opt => opt.MapFrom(src => src.AssignedCoach != null ? src.AssignedCoach.Username : null));
            CreateMap<CreatePackageRequestDto, Package>()
                .ForMember(dest => dest.PackageId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Creator, opt => opt.Ignore())
                .ForMember(dest => dest.Plans, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore());
            CreateMap<UpdatePackageRequestDto, Package>();

            // Plan mappings
            CreateMap<Plan, PlanDto>()
                .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Package.Name))
                .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.Username))
                .ForMember(dest => dest.CoachName, opt => opt.MapFrom(src => src.Coach != null ? src.Coach.Username : null));
            CreateMap<CreatePlanRequestDto, Plan>();
            CreateMap<UpdatePlanRequestDto, Plan>();

            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
            CreateMap<CreateOrderRequestDto, Order>();

            // CoachCommission mappings
            CreateMap<CoachCommission, CoachCommissionDto>()
                .ForMember(dest => dest.CoachName, opt => opt.MapFrom(src => src.Coach.Username));
        }
    }
} 