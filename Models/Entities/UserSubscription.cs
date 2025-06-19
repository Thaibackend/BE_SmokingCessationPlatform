using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmokingQuitSupportAPI.Constants;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class UserSubscription
    {
        [Key]
        public int SubscriptionId { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        [StringLength(20)]
        public string PackageType { get; set; } = PackageTypes.BASIC;
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = SubscriptionStatus.ACTIVE;
        
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Price { get; set; }
        
        public int? AssignedCoachId { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Account Account { get; set; } = null!;
        public virtual Coach? AssignedCoach { get; set; }
    }
} 