using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class CommunityPost
    {
        [Key]
        public int PostId { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? Category { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Published";
        
        public int ViewCount { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    }
} 