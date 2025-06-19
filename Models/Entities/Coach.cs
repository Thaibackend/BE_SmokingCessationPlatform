using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Coach
    {
        [Key]
        public int CoachId { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        
        [StringLength(1000)]
        public string? Qualifications { get; set; }
        
        [StringLength(1000)]
        public string? Experience { get; set; }
        
        [StringLength(1000)]
        public string? Bio { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";
        
        public int Rating { get; set; } = 0;
        public int SessionsCompleted { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<CoachSession> CoachSessions { get; set; } = new List<CoachSession>();
        public virtual ICollection<MemberPackage> AssignedPackages { get; set; } = new List<MemberPackage>();
        public virtual ICollection<QuitPlan> QuitPlans { get; set; } = new List<QuitPlan>();
    }
} 