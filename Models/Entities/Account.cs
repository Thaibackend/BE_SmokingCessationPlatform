using System.ComponentModel.DataAnnotations;
using SmokingQuitSupportAPI.Constants;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? FullName { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = Roles.User;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Coach? Coach { get; set; }
        public virtual ICollection<CoachSession> CoachSessions { get; set; } = new List<CoachSession>();
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public virtual ICollection<CommunityPost> CommunityPosts { get; set; } = new List<CommunityPost>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
        public virtual ICollection<MemberPackage> CreatedPackages { get; set; } = new List<MemberPackage>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<QuitPlan> QuitPlans { get; set; } = new List<QuitPlan>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<SmokingStatus> SmokingStatuses { get; set; } = new List<SmokingStatus>();
        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
        public virtual ICollection<Progress> ProgressRecords { get; set; } = new List<Progress>();
    }
} 