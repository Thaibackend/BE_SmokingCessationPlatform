using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        
        public int? CoachSessionId { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = "Session";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual Account Account { get; set; } = null!;
        public virtual CoachSession? CoachSession { get; set; }
    }
} 