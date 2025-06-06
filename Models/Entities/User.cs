using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được quá 50 ký tự")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được quá 100 ký tự")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string PasswordHash { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự")]
        public string? FullName { get; set; }
        
        [StringLength(20)]
        public string Role { get; set; } = "User";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Appointment> MemberAppointments { get; set; } = new List<Appointment>();
        public ICollection<Appointment> CoachAppointments { get; set; } = new List<Appointment>();
        public ICollection<Plan> MemberPlans { get; set; } = new List<Plan>();
        public ICollection<Plan> CoachPlans { get; set; } = new List<Plan>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
        public ICollection<SmokingStatus> SmokingStatuses { get; set; } = new List<SmokingStatus>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<DailyTask> DailyTasks { get; set; } = new List<DailyTask>();
        public ICollection<CoachApplication> CoachApplications { get; set; } = new List<CoachApplication>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<CoachCommission> CoachCommissions { get; set; } = new List<CoachCommission>();
        public ICollection<Package> AssignedPackages { get; set; } = new List<Package>();

        // Helper method để check role
        public bool IsAdmin() => Role == "Admin";
        public bool IsCoach() => Role == "Coach" || IsAdmin();
        public bool IsUser() => Role == "User";
    }
} 