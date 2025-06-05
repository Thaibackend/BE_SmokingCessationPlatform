using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public int PostId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Post Post { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
} 