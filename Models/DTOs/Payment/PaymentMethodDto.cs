using System;

namespace SmokingQuitSupportAPI.Models.DTOs.Payment
{
    public class PaymentMethodDto
    {
        public int PaymentMethodId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 