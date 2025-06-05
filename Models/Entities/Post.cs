using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public int UserId { get; set; }
        public string Category { get; set; } = string.Empty; // Tips, Experience, Question
        public string Type { get; set; } = "Post"; // Post, Comment
        public int? ParentId { get; set; } // Cho comment, null nếu là post
        public string Status { get; set; } = "Published";
        public int ViewCount { get; set; } = 0;
        public int LikeCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Post? Parent { get; set; }
        public virtual ICollection<Post> Comments { get; set; } = new List<Post>();
        public virtual ICollection<PostInteraction> Interactions { get; set; } = new List<PostInteraction>();
    }
} 