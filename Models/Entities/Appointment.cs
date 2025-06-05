using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        
        public int MemberId { get; set; }
        public int CoachId { get; set; }
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Scheduled";
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual User Member { get; set; } = null!;
        public virtual User Coach { get; set; } = null!;
    }
} 