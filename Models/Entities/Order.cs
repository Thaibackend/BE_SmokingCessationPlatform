using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmokingQuitSupportAPI.Models.Entities
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        public int PackageId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
        
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        public string? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal CoachCommission { get; set; } = 0;
        
        public bool IsCommissionPaid { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Account Account { get; set; } = null!;
        public virtual MemberPackage Package { get; set; } = null!;
    }
} 