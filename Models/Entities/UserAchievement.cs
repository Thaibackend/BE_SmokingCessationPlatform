using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class UserAchievement
    {
        [Key]
        public int UserAchievementId { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        public int AchievementId { get; set; }
        
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Account Account { get; set; } = null!;
        public virtual Achievement Achievement { get; set; } = null!;
    }
} 