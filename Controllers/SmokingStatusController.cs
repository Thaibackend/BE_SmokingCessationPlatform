using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.SmokingStatus;
using SmokingQuitSupportAPI.Services.Interfaces;
using System.Security.Claims;

namespace SmokingQuitSupportAPI.Controllers
{
    /// <summary>
    /// Controller cho tình trạng hút thuốc - Ghi nhận thông tin hút thuốc hiện tại
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SmokingStatusController : ControllerBase
    {
        private readonly ISmokingStatusService _smokingStatusService;

        public SmokingStatusController(ISmokingStatusService smokingStatusService)
        {
            _smokingStatusService = smokingStatusService;
        }

        /// <summary>
        /// Lấy tình trạng hút thuốc của tôi
        /// </summary>
        [HttpGet("my-status")]
        public async Task<ActionResult<SmokingStatusDto>> GetMySmokingStatus()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var status = await _smokingStatusService.GetSmokingStatusAsync(accountId);
                
                if (status == null)
                {
                    return NotFound(new { message = "Chưa có thông tin tình trạng hút thuốc" });
                }

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy tình trạng hút thuốc", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo hoặc cập nhật tình trạng hút thuốc
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SmokingStatusDto>> CreateOrUpdateSmokingStatus([FromBody] CreateSmokingStatusDto createStatusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var status = await _smokingStatusService.CreateOrUpdateSmokingStatusAsync(accountId, createStatusDto);
                
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật tình trạng hút thuốc", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thống kê tiến trình
        /// </summary>
        [HttpPost("update-statistics")]
        public async Task<ActionResult> UpdateProgressStatistics()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _smokingStatusService.UpdateProgressStatisticsAsync(accountId);
                
                return Ok(new { message = "Cập nhật thống kê thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật thống kê", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy bảng xếp hạng theo tiền tiết kiệm
        /// </summary>
        [HttpGet("leaderboard/money-saved")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SmokingStatusDto>>> GetLeaderboardByMoneySaved([FromQuery] int take = 10)
        {
            try
            {
                var leaderboard = await _smokingStatusService.GetLeaderboardByMoneySavedAsync(take);
                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy bảng xếp hạng", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy bảng xếp hạng theo số ngày không hút thuốc
        /// </summary>
        [HttpGet("leaderboard/smoke-free-days")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SmokingStatusDto>>> GetLeaderboardBySmokeFreesDays([FromQuery] int take = 10)
        {
            try
            {
                var leaderboard = await _smokingStatusService.GetLeaderboardBySmokeFreesDaysAsync(take);
                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy bảng xếp hạng", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quan của hệ thống
        /// </summary>
        [HttpGet("overall-statistics")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetOverallStatistics()
        {
            try
            {
                var statistics = await _smokingStatusService.GetOverallStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê tổng quan", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy chỉ số Brinkman Index của tôi với dữ liệu biểu đồ sóng
        /// </summary>
        [HttpGet("brinkman-index")]
        public async Task<ActionResult<BrinkmanIndexDto>> GetMyBrinkmanIndex()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var brinkmanIndex = await _smokingStatusService.GetBrinkmanIndexAsync(accountId);
                
                if (brinkmanIndex == null)
                {
                    return NotFound(new { message = "Chưa có thông tin tình trạng hút thuốc để tính chỉ số Brinkman" });
                }

                return Ok(brinkmanIndex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy chỉ số Brinkman Index", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê Brinkman Index tổng quan của hệ thống
        /// </summary>
        [HttpGet("brinkman-statistics")]
        [AllowAnonymous]
        public async Task<ActionResult<BrinkmanStatisticsDto>> GetBrinkmanStatistics()
        {
            try
            {
                var statistics = await _smokingStatusService.GetBrinkmanStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê Brinkman Index", error = ex.Message });
            }
        }
    }
} 