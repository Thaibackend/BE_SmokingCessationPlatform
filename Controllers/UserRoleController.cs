using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.Role;
using SmokingQuitSupportAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmokingQuitSupportAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserRoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public UserRoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Get roles for a specific user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetUserRoles(int userId)
        {
            var roles = await _roleService.GetUserRolesAsync(userId);
            return Ok(roles);
        }

        /// <summary>
        /// Assign roles to a user
        /// </summary>
        [HttpPost("user/{userId}")]
        public async Task<IActionResult> AssignRolesToUser(int userId, [FromBody] AssignRolesToUserRequest request)
        {
            await _roleService.AssignRolesToUserAsync(userId, request.RoleIds);
            return NoContent();
        }

        /// <summary>
        /// Get current user's roles
        /// </summary>
        [HttpGet("my-roles")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetMyRoles()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var roles = await _roleService.GetUserRolesAsync(userId);
            return Ok(roles);
        }
    }
} 