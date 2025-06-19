using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class QuitPlan
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
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [Required]
        public int PackageId { get; set; }
        
        [Required]
        public int MemberId { get; set; }
        
        public int? CoachId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual MemberPackage Package { get; set; } = null!;
        public virtual Account Member { get; set; } = null!;
        public virtual Coach? Coach { get; set; }
    }
} 