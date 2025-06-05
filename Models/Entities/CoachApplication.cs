using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class CoachApplication
    {
        [Key]
        public int ApplicationId { get; set; }
        
        public int UserId { get; set; }
        
        [StringLength(1000)]
        public string? Qualifications { get; set; }
        
        [StringLength(1000)]
        public string? Experience { get; set; }
        
        [StringLength(1000)]
        public string? Motivation { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
        
        [StringLength(1000)]
        public string? ReviewNotes { get; set; }
        
        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedDate { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
    }
} 