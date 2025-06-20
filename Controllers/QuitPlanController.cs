using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.QuitPlan;
using SmokingQuitSupportAPI.Services.Interfaces;
using System.Security.Claims;

namespace SmokingQuitSupportAPI.Controllers
{
    /// <summary>
    /// Controller cho kế hoạch cai thuốc
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuitPlanController : ControllerBase
    {
        private readonly IQuitPlanService _quitPlanService;

        public QuitPlanController(IQuitPlanService quitPlanService)
        {
            _quitPlanService = quitPlanService;
        }

        /// <summary>
        /// Lấy tất cả kế hoạch cai thuốc của tôi
        /// </summary>
        [HttpGet("my-plans")]
        public async Task<ActionResult<IEnumerable<QuitPlanDto>>> GetMyQuitPlans()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var plans = await _quitPlanService.GetUserQuitPlansAsync(accountId);
                return Ok(plans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách kế hoạch", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy kế hoạch cai thuốc đang hoạt động
        /// </summary>
        [HttpGet("active-plan")]
        public async Task<ActionResult<QuitPlanDto>> GetActiveQuitPlan()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var plan = await _quitPlanService.GetActiveQuitPlanAsync(accountId);
                
                if (plan == null)
                {
                    return NotFound(new { message = "Không có kế hoạch cai thuốc đang hoạt động" });
                }

                return Ok(plan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy kế hoạch đang hoạt động", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy kế hoạch cai thuốc theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<QuitPlanDto>> GetQuitPlanById(int id)
        {
            try
            {
                var plan = await _quitPlanService.GetQuitPlanByIdAsync(id);
                if (plan == null)
                {
                    return NotFound(new { message = "Không tìm thấy kế hoạch cai thuốc" });
                }

                return Ok(plan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy kế hoạch cai thuốc", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo kế hoạch cai thuốc mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<QuitPlanDto>> CreateQuitPlan([FromBody] CreateQuitPlanDto createPlanDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var plan = await _quitPlanService.CreateQuitPlanAsync(accountId, createPlanDto);
                
                return CreatedAtAction(nameof(GetQuitPlanById), new { id = plan.PlanId }, plan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo kế hoạch cai thuốc", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo kế hoạch cai thuốc tự động
        /// </summary>
        [HttpPost("generate-automatic")]
        public async Task<ActionResult<QuitPlanDto>> GenerateAutomaticQuitPlan([FromBody] int packageId)
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var plan = await _quitPlanService.GenerateAutomaticQuitPlanAsync(accountId, packageId);
                
                return CreatedAtAction(nameof(GetQuitPlanById), new { id = plan.PlanId }, plan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo kế hoạch tự động", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật kế hoạch cai thuốc
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<QuitPlanDto>> UpdateQuitPlan(int id, [FromBody] CreateQuitPlanDto updatePlanDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var plan = await _quitPlanService.UpdateQuitPlanAsync(id, accountId, updatePlanDto);
                
                if (plan == null)
                {
                    return NotFound(new { message = "Không tìm thấy kế hoạch hoặc bạn không có quyền chỉnh sửa" });
                }

                return Ok(plan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật kế hoạch", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái kế hoạch
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdatePlanStatus(int id, [FromBody] string status)
        {
            try
            {
                var success = await _quitPlanService.UpdatePlanStatusAsync(id, status);
                if (!success)
                {
                    return NotFound(new { message = "Không tìm thấy kế hoạch" });
                }

                return Ok(new { message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật trạng thái", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa kế hoạch cai thuốc
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuitPlan(int id)
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var success = await _quitPlanService.DeleteQuitPlanAsync(id, accountId);
                
                if (!success)
                {
                    return NotFound(new { message = "Không tìm thấy kế hoạch hoặc bạn không có quyền xóa" });
                }

                return Ok(new { message = "Xóa kế hoạch thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa kế hoạch", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê kế hoạch cai thuốc
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetQuitPlanStatistics()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var statistics = await _quitPlanService.GetQuitPlanStatisticsAsync(accountId);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê kế hoạch", error = ex.Message });
            }
        }
    }
} 