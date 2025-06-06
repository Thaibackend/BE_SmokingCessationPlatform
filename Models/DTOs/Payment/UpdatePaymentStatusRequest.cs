using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.DTOs.Payment
{
    public class UpdatePaymentStatusRequest
    {
        [Required]
        public string Status { get; set; }

        public string TransactionId { get; set; }
        
        public string ResponseData { get; set; }
    }
} 