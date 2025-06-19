using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class SmokingStatus
    {
        [Key]
        public int StatusId { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        public DateTime QuitDate { get; set; }
        
        [Required]
        public int CigarettesPerDay { get; set; }
        
        [Required]
        public int YearsOfSmoking { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CostPerPack { get; set; }
        
        [Required]
        public int CigarettesPerPack { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal MoneySaved { get; set; } = 0;
        
        public int SmokeFreenDays { get; set; } = 0;
        public int CigarettesAvoided { get; set; } = 0;
        
        [StringLength(500)]
        public string? HealthImprovement { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Account Account { get; set; } = null!;
    }
} 