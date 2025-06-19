using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class MemberPackage
    {
        [Key]
        public int PackageId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [StringLength(50)]
        public string? Type { get; set; }
        
        [Required]
        public int DurationDays { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int? CreatedById { get; set; }
        public int? AssignedCoachId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Account? Creator { get; set; }
        public virtual Coach? AssignedCoach { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<QuitPlan> QuitPlans { get; set; } = new List<QuitPlan>();
    }
} 