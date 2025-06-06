namespace SmokingQuitSupportAPI.Models.DTOs.Payment
{
    public class PaymentVerificationResult
    {
        public bool IsVerified { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public PaymentDto Payment { get; set; }
    }
} 