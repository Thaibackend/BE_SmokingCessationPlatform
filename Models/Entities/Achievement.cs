using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Achievement
    {
        [Key]
        public int AchievementId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(200)]
        public string? Icon { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;
        
        public int RequiredValue { get; set; } = 0;
        
        [StringLength(20)]
        public string? BadgeColor { get; set; }
        
        public int Points { get; set; } = 0;
        
        [StringLength(500)]  
        public string? Criteria { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
    }
} 