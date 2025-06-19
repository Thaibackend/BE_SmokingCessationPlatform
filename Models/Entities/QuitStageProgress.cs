using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmokingQuitSupportAPI.Constants;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class QuitStageProgress
    {
        [Key]
        public int StageProgressId { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string CurrentStage { get; set; } = QuitStages.PREPARATION;
        
        public DateTime StageStartDate { get; set; } = DateTime.UtcNow;
        public DateTime? StageEndDate { get; set; }
        
        [Range(0, 100)]
        public int ProgressPercentage { get; set; } = 0;
        
        [StringLength(1000)]
        public string? StageGoals { get; set; }
        
        [StringLength(2000)]
        public string? CoachNotes { get; set; }
        
        [StringLength(2000)]
        public string? UserNotes { get; set; }
        
        // Metrics cho từng giai đoạn
        public int? CigarettesSmoked { get; set; } = 0;
        public int? CravingLevel { get; set; } // 1-10
        public int? StressLevel { get; set; } // 1-10
        public int? SupportLevel { get; set; } // 1-10
        
        [StringLength(1000)]
        public string? Challenges { get; set; }
        
        [StringLength(1000)]
        public string? Achievements { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Account Account { get; set; } = null!;
    }
} 