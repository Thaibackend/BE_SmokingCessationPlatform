using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class UserAchievement
    {
        [Key]
        public int UserAchievementId { get; set; }
        
        public int UserId { get; set; }
        public int AchievementId { get; set; }
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Achievement Achievement { get; set; } = null!;
    }
} 