using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class SmokingStatus
    {
        [Key]
        public int StatusId { get; set; }
        
        public int UserId { get; set; }
        public DateTime QuitDate { get; set; }
        public int CigarettesPerDay { get; set; }
        public decimal CostPerPack { get; set; }
        public int CigarettesPerPack { get; set; }
        public decimal MoneySaved { get; set; }
        public int DaysSmokeFree { get; set; }
        public int CigarettesAvoided { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "Active";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual User User { get; set; } = null!;
    }
} 