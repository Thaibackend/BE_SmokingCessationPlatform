using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class CoachSession
    {
        [Key]
        public int SessionId { get; set; }
        
        [Required]
        public int CoachId { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        public DateTime SessionDate { get; set; }
        
        [Required]
        public DateTime StartTime { get; set; }
        
        [Required]
        public DateTime EndTime { get; set; }
        
        [StringLength(50)]
        public string? SessionType { get; set; } = "CONSULTATION";
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Scheduled";
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual Coach Coach { get; set; } = null!;
        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
} 