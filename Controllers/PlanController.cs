using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.Plan;
using SmokingQuitSupportAPI.Services;
using System.Security.Claims;

namespace SmokingQuitSupportAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlanController : ControllerBase
    {
        private readonly PlanService _planService;

        public PlanController(PlanService planService)
        {
            _planService = planService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlan([FromBody] CreatePlanRequestDto request)
        {
            try
            {
                var plan = await _planService.CreatePlanAsync(request);
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPlans()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var plans = await _planService.GetUserPlansAsync(userId);
                return Ok(plans);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("coach")]
        public async Task<IActionResult> GetCoachPlans()
        {
            try
            {
                var coachId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var plans = await _planService.GetCoachPlansAsync(coachId);
                return Ok(plans);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlan(int id)
        {
            try
            {
                var plan = await _planService.GetPlanAsync(id);
                if (plan == null)
                    return NotFound();
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdatePlanStatus(int id, [FromBody] string status)
        {
            try
            {
                await _planService.UpdatePlanStatusAsync(id, status);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            try
            {
                await _planService.DeletePlanAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 