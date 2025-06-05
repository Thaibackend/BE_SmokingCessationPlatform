using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class DailyTask
    {
        [Key]
        public int TaskId { get; set; }
        
        public int UserId { get; set; }
        public int? PlanId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public DateTime DueDate { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
        
        [StringLength(20)]
        public string Priority { get; set; } = "Medium";
        
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Plan? Plan { get; set; }
    }
} 