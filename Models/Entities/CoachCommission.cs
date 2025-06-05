using System;
using System.ComponentModel.DataAnnotations;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class CoachCommission
    {
        [Key]
        public int CommissionId { get; set; }
        public int CoachId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidDate { get; set; }
        public string Status { get; set; } = "Pending"; // "Pending", "Paid"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User Coach { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
} 