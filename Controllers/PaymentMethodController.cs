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
    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService;

        public PaymentMethodController(IPaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }

        /// <summary>
        /// Get all active payment methods
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetPaymentMethods()
        {
            var methods = await _paymentMethodService.GetPaymentMethodsAsync();
            return Ok(methods);
        }

        /// <summary>
        /// Get a specific payment method by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentMethodDto>> GetPaymentMethod(int id)
        {
            var method = await _paymentMethodService.GetPaymentMethodByIdAsync(id);
            return Ok(method);
        }

        /// <summary>
        /// Create a new payment method (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaymentMethodDto>> CreatePaymentMethod([FromBody] CreatePaymentMethodRequest request)
        {
            var method = await _paymentMethodService.CreatePaymentMethodAsync(request);
            return CreatedAtAction(nameof(GetPaymentMethod), new { id = method.PaymentMethodId }, method);
        }

        /// <summary>
        /// Update an existing payment method (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaymentMethodDto>> UpdatePaymentMethod(int id, [FromBody] UpdatePaymentMethodRequest request)
        {
            var method = await _paymentMethodService.UpdatePaymentMethodAsync(id, request);
            return Ok(method);
        }

        /// <summary>
        /// Toggle the active status of a payment method (Admin only)
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaymentMethodDto>> TogglePaymentMethodStatus(int id)
        {
            var method = await _paymentMethodService.TogglePaymentMethodStatusAsync(id);
            return Ok(method);
        }
    }
} 