using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Plan
    {
        [Key]
        public int PlanId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";
        
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int PackageId { get; set; }
        public int MemberId { get; set; }
        public int? CoachId { get; set; }

        // Navigation Properties
        public virtual Package Package { get; set; } = null!;
        public virtual User Member { get; set; } = null!;
        public virtual User? Coach { get; set; }
        
        // New navigation property
        public virtual ICollection<DailyTask> DailyTasks { get; set; } = new List<DailyTask>();
    }
} 