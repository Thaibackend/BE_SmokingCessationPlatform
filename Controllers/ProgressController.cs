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
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
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
        /// Lấy tiến trình theo ngày
        /// </summary>
        [HttpGet("by-date")]
        public async Task<ActionResult<ProgressDto>> GetProgressByDate([FromQuery] DateTime date)
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var progress = await _progressService.GetProgressByDateAsync(accountId, date);
                
                if (progress == null)
                {
                    return NotFound(new { message = "Không có dữ liệu tiến trình cho ngày này" });
                }

                return Ok(progress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy tiến trình theo ngày", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy tiến trình trong khoảng thời gian
        /// </summary>
        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<ProgressDto>>> GetProgressByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var progress = await _progressService.GetProgressByDateRangeAsync(accountId, startDate, endDate);
                return Ok(progress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy tiến trình theo khoảng thời gian", error = ex.Message });
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
        /// Cập nhật tiến trình
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProgressDto>> UpdateProgress(int id, [FromBody] CreateProgressDto updateProgressDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var progress = await _progressService.UpdateProgressAsync(id, accountId, updateProgressDto);
                
                if (progress == null)
                {
                    return NotFound(new { message = "Không tìm thấy tiến trình hoặc bạn không có quyền chỉnh sửa" });
                }

                return Ok(progress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật tiến trình", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa ghi nhận tiến trình
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProgress(int id)
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var success = await _progressService.DeleteProgressAsync(id, accountId);
                
                if (!success)
                {
                    return NotFound(new { message = "Không tìm thấy tiến trình hoặc bạn không có quyền xóa" });
                }

                return Ok(new { message = "Xóa ghi nhận tiến trình thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa tiến trình", error = ex.Message });
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

        /// <summary>
        /// Lấy streak dài nhất
        /// </summary>
        [HttpGet("longest-streak")]
        public async Task<ActionResult<int>> GetLongestStreak()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var streak = await _progressService.GetLongestStreakAsync(accountId);
                return Ok(new { longestStreak = streak });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy streak dài nhất", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thống kê tự động
        /// </summary>
        [HttpPost("update-automatic-statistics")]
        public async Task<ActionResult> UpdateAutomaticStatistics()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _progressService.UpdateAutomaticStatisticsAsync(accountId);
                
                return Ok(new { message = "Cập nhật thống kê tự động thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật thống kê tự động", error = ex.Message });
            }
        }
    }
} 