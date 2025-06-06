using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.Payment;
using SmokingQuitSupportAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmokingQuitSupportAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Get all payments (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments([FromQuery] PaymentFilterRequest filter)
        {
            var payments = await _paymentService.GetPaymentsAsync(filter);
            return Ok(payments);
        }

        /// <summary>
        /// Get a specific payment by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PaymentDto>> GetPayment(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var payment = await _paymentService.GetPaymentByIdAsync(id, userId);
            return Ok(payment);
        }

        /// <summary>
        /// Process a new payment
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PaymentDto>> ProcessPayment([FromBody] ProcessPaymentRequest request)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var payment = await _paymentService.ProcessPaymentAsync(userId, request);
            return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentId }, payment);
        }

        /// <summary>
        /// Update payment status
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaymentDto>> UpdatePaymentStatus(int id, [FromBody] UpdatePaymentStatusRequest request)
        {
            var payment = await _paymentService.UpdatePaymentStatusAsync(id, request);
            return Ok(payment);
        }

        /// <summary>
        /// Get payments for the current user
        /// </summary>
        [HttpGet("my-payments")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetMyPayments([FromQuery] PaymentFilterRequest filter)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var payments = await _paymentService.GetUserPaymentsAsync(userId, filter);
            return Ok(payments);
        }

        /// <summary>
        /// Verify a payment
        /// </summary>
        [HttpPost("verify")]
        [Authorize]
        public async Task<ActionResult<PaymentVerificationResult>> VerifyPayment([FromBody] PaymentVerificationRequest request)
        {
            var result = await _paymentService.VerifyPaymentAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Get payment methods
        /// </summary>
        [HttpGet("methods")]
        public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetPaymentMethods()
        {
            var methods = await _paymentService.GetPaymentMethodsAsync();
            return Ok(methods);
        }
    }
} 