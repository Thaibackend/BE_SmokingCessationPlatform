namespace SmokingQuitSupportAPI.Models.DTOs.Payment
{
    public class PaymentVerificationRequest
    {
        public string TransactionId { get; set; }
        public int? OrderId { get; set; }
        public string PaymentProvider { get; set; }
        public string VerificationData { get; set; }
    }
} 