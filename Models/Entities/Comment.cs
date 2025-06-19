using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        
        [Required]
        public int PostId { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public int? ParentId { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Published";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual CommunityPost Post { get; set; } = null!;
        public virtual Account Account { get; set; } = null!;
        public virtual Comment? Parent { get; set; }
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
} 