using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.Achievement;
using SmokingQuitSupportAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmokingQuitSupportAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserAchievementController : ControllerBase
    {
        private readonly IUserAchievementService _userAchievementService;

        public UserAchievementController(IUserAchievementService userAchievementService)
        {
            _userAchievementService = userAchievementService;
        }

        /// <summary>
        /// Get current user's achievements
        /// </summary>
        [HttpGet("my")]
        [ProducesResponseType(typeof(IEnumerable<UserAchievementDto>), 200)]
        public async Task<IActionResult> GetMyAchievements()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var achievements = await _userAchievementService.GetUserAchievementsAsync(userId);
            return Ok(achievements);
        }

        /// <summary>
        /// Get current user's achievement progress
        /// </summary>
        [HttpGet("my/progress")]
        [ProducesResponseType(typeof(IEnumerable<AchievementProgressDto>), 200)]
        public async Task<IActionResult> GetMyAchievementProgress()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var progress = await _userAchievementService.GetAchievementProgressAsync(userId);
            return Ok(progress);
        }

        /// <summary>
        /// Check and award achievements to current user
        /// </summary>
        [HttpPost("check")]
        [ProducesResponseType(typeof(IEnumerable<UserAchievementDto>), 200)]
        public async Task<IActionResult> CheckAndAwardAchievements()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var newAchievements = await _userAchievementService.CheckAndAwardQualifiedAchievementsAsync(userId);
            return Ok(newAchievements);
        }

        /// <summary>
        /// Award an achievement to a user (admin only)
        /// </summary>
        [HttpPost("award")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserAchievementDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AwardAchievement([FromBody] AwardAchievementRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userAchievement = await _userAchievementService.AwardAchievementAsync(request);
            return Ok(userAchievement);
        }

        /// <summary>
        /// Revoke an achievement from a user (admin only)
        /// </summary>
        [HttpDelete("{userAchievementId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RevokeAchievement(int userAchievementId)
        {
            await _userAchievementService.RevokeAchievementAsync(userAchievementId);
            return NoContent();
        }

        /// <summary>
        /// Get achievements for a specific user (admin only)
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<UserAchievementDto>), 200)]
        public async Task<IActionResult> GetUserAchievements(int userId)
        {
            var achievements = await _userAchievementService.GetUserAchievementsAsync(userId);
            return Ok(achievements);
        }

        /// <summary>
        /// Get achievement progress for a specific user (admin only)
        /// </summary>
        [HttpGet("user/{userId}/progress")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<AchievementProgressDto>), 200)]
        public async Task<IActionResult> GetUserAchievementProgress(int userId)
        {
            var progress = await _userAchievementService.GetAchievementProgressAsync(userId);
            return Ok(progress);
        }
    }
} 