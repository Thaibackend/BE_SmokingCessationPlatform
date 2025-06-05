using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int PackageId { get; set; }
        public decimal Amount { get; set; }
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // "Pending", "Paid", "Cancelled"
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Thêm commission cho coach
        public decimal CoachCommission { get; set; }
        public bool IsCommissionPaid { get; set; } = false;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Package Package { get; set; } = null!;
    }
} 