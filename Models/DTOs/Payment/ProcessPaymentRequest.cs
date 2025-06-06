using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.DTOs.Payment
{
    public class ProcessPaymentRequest
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int PaymentMethodId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string TransactionId { get; set; }

        public string AdditionalData { get; set; }
    }
} 