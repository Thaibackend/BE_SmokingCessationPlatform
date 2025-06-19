using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Progress
    {
        [Key]
        public int ProgressId { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        public int SmokeFreenDays { get; set; } = 0;
        public int CigarettesAvoided { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal MoneySaved { get; set; } = 0;
        
        public int? HealthScore { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        public int? Mood { get; set; }
        public int? CravingLevel { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Weight { get; set; }
        
        public int? ExerciseMinutes { get; set; }
        
        [Column(TypeName = "decimal(4,2)")]
        public decimal? SleepHours { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Account Account { get; set; } = null!;
    }
} 