using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.Achievement;
using SmokingQuitSupportAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmokingQuitSupportAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementController : ControllerBase
    {
        private readonly IAchievementService _achievementService;

        public AchievementController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        /// <summary>
        /// Get all achievements
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AchievementDto>), 200)]
        public async Task<IActionResult> GetAllAchievements()
        {
            var achievements = await _achievementService.GetAllAchievementsAsync();
            return Ok(achievements);
        }

        /// <summary>
        /// Get achievement by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AchievementDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAchievementById(int id)
        {
            var achievement = await _achievementService.GetAchievementByIdAsync(id);
            if (achievement == null)
                return NotFound();

            return Ok(achievement);
        }

        /// <summary>
        /// Create a new achievement
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AchievementDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAchievement([FromBody] CreateAchievementRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var achievement = await _achievementService.CreateAchievementAsync(request);
            return CreatedAtAction(nameof(GetAchievementById), new { id = achievement.AchievementId }, achievement);
        }

        /// <summary>
        /// Update an existing achievement
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AchievementDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateAchievement(int id, [FromBody] UpdateAchievementRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var achievement = await _achievementService.UpdateAchievementAsync(id, request);
            if (achievement == null)
                return NotFound();

            return Ok(achievement);
        }

        /// <summary>
        /// Delete an achievement
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAchievement(int id)
        {
            var success = await _achievementService.DeleteAchievementAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
} 