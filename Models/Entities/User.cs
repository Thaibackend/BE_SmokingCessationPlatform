using System.ComponentModel.DataAnnotations;
using SmokingQuitSupportAPI.Models.Enums;

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
        public string Role { get; set; } = UserRole.User.ToString();
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Existing navigation properties
        public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Appointment> MemberAppointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Appointment> CoachAppointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Plan> MemberPlans { get; set; } = new List<Plan>();
        public virtual ICollection<Plan> CoachPlans { get; set; } = new List<Plan>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        // New navigation properties
        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
        public virtual ICollection<SmokingStatus> SmokingStatuses { get; set; } = new List<SmokingStatus>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<DailyTask> DailyTasks { get; set; } = new List<DailyTask>();
        public virtual ICollection<CoachApplication> CoachApplications { get; set; } = new List<CoachApplication>();

        // Helper method để check role
        public bool IsAdmin() => Role == UserRole.Admin.ToString();
        public bool IsCoach() => Role == UserRole.Coach.ToString() || IsAdmin();
        public bool IsUser() => Role == UserRole.User.ToString();
    }
} 