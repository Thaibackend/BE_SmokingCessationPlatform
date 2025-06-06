using System;

namespace SmokingQuitSupportAPI.Models.DTOs.Payment
{
    public class PaymentFilterRequest
    {
        public int? OrderId { get; set; }
        public int? UserId { get; set; }
        public int? PaymentMethodId { get; set; }
        public string Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
} 