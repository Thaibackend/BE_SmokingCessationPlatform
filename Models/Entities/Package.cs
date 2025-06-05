using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Package
    {
        [Key]
        public int PackageId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [StringLength(50)]
        public string Type { get; set; } = string.Empty; // "basic", "premium", "vip"
        
        public int DurationDays { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Thêm trường để track coach
        public int? AssignedCoachId { get; set; }
        public virtual User? AssignedCoach { get; set; }

        // Navigation properties
        public virtual ICollection<Plan> Plans { get; set; } = new List<Plan>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public int? CreatedById { get; set; }
        public virtual User? Creator { get; set; }
    }
} 