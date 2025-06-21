using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.Progress;
using SmokingQuitSupportAPI.Services.Interfaces;
using System.Security.Claims;

namespace SmokingQuitSupportAPI.Controllers
{
    /// <summary>
    /// Controller cho tiến trình cai thuốc - Ghi nhận và thống kê tiến độ
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DailyProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public DailyProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        /// <summary>
        /// Lấy tất cả tiến trình của tôi
        /// </summary>
        [HttpGet("my-progress")]
        public async Task<ActionResult<IEnumerable<ProgressDto>>> GetMyProgress()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var progress = await _progressService.GetUserProgressAsync(accountId);
                return Ok(progress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy tiến trình", error = ex.Message });
            }
        }

        /// <summary>
        /// Ghi nhận tiến trình hàng ngày
        /// </summary>
        [HttpPost("daily")]
        public async Task<ActionResult<ProgressDto>> RecordDailyProgress([FromBody] CreateProgressDto createProgressDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var progress = await _progressService.RecordDailyProgressAsync(accountId, createProgressDto);
                
                return Ok(progress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi ghi nhận tiến trình", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê tiến trình tổng quan
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetProgressStatistics()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var statistics = await _progressService.GetProgressStatisticsAsync(accountId);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê tiến trình", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy streak hiện tại (chuỗi ngày liên tiếp không hút thuốc)
        /// </summary>
        [HttpGet("current-streak")]
        public async Task<ActionResult<int>> GetCurrentStreak()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var streak = await _progressService.GetCurrentStreakAsync(accountId);
                return Ok(new { currentStreak = streak });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy streak hiện tại", error = ex.Message });
            }
        }
    }
} 