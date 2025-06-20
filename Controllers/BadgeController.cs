using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.Achievement;
using SmokingQuitSupportAPI.Services.Interfaces;
using System.Security.Claims;

namespace SmokingQuitSupportAPI.Controllers
{
    /// <summary>
    /// Controller cho huy hiệu thành tích
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BadgeController : ControllerBase
    {
        private readonly IAchievementService _achievementService;

        public BadgeController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        /// <summary>
        /// Lấy tất cả huy hiệu thành tích
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AchievementDto>>> GetAllAchievements()
        {
            try
            {
                var achievements = await _achievementService.GetAllAchievementsAsync();
                return Ok(achievements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách huy hiệu", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy huy hiệu thành tích của tôi
        /// </summary>
        [HttpGet("my-badges")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AchievementDto>>> GetMyAchievements()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var achievements = await _achievementService.GetUserAchievementsAsync(accountId);
                return Ok(achievements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy huy hiệu của bạn", error = ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra và mở khóa huy hiệu thành tích mới
        /// </summary>
        [HttpPost("check-unlock")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AchievementDto>>> CheckAndUnlockAchievements()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var newAchievements = await _achievementService.CheckAndUnlockAchievementsAsync(accountId);
                
                return Ok(new { 
                    message = $"Đã kiểm tra và mở khóa {newAchievements.Count()} huy hiệu mới",
                    newAchievements = newAchievements 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi kiểm tra huy hiệu", error = ex.Message });
            }
        }

        /// <summary>
        /// Chia sẻ huy hiệu thành tích
        /// </summary>
        [HttpPost("{id}/share")]
        [Authorize]
        public async Task<ActionResult> ShareAchievement(int id)
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var success = await _achievementService.ShareAchievementAsync(accountId, id);
                
                if (!success)
                {
                    return BadRequest(new { message = "Không thể chia sẻ huy hiệu này" });
                }

                return Ok(new { message = "Chia sẻ huy hiệu thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi chia sẻ huy hiệu", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy bảng xếp hạng theo số huy hiệu
        /// </summary>
        [HttpGet("leaderboard")]
        public async Task<ActionResult<IEnumerable<object>>> GetAchievementLeaderboard([FromQuery] int take = 10)
        {
            try
            {
                var leaderboard = await _achievementService.GetAchievementLeaderboardAsync(take);
                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy bảng xếp hạng", error = ex.Message });
            }
        }
    }
} 