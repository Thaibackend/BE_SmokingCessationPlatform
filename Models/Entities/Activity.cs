using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Activity
    {
        [Key]
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } = string.Empty; // "quit_attempt", "relapse", "milestone", "exercise"
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int CigarettesSmoked { get; set; }
        public decimal MoneySaved { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User User { get; set; } = null!;
    }
} 