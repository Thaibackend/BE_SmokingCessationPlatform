using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class PostLike
    {
        [Key]
        public int LikeId { get; set; }
        
        [Required]
        public int PostId { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual CommunityPost Post { get; set; } = null!;
        public virtual Account Account { get; set; } = null!;
    }
} 