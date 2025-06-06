using System;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethodId { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string ResponseData { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Order Order { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
} 