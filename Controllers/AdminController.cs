using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Attributes;
using SmokingQuitSupportAPI.Data;
using SmokingQuitSupportAPI.Models.DTOs.Account;
using SmokingQuitSupportAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Constants;

namespace SmokingQuitSupportAPI.Controllers
{
    /// <summary>
    /// Controller dành cho Admin quản lý hệ thống
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AdminRequired]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public AdminController(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        /// <summary>
        /// Lấy danh sách tất cả accounts
        /// </summary>
        [HttpGet("accounts")]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAllAccounts()
        {
            try
            {
                var accounts = await _context.Accounts
                    .Select(a => new AccountDto
                    {
                        AccountId = a.AccountId,
                        Username = a.Username,
                        Email = a.Email,
                        FullName = a.FullName,
                        Role = a.Role,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách accounts", error = ex.Message });
            }
        }

        /// <summary>
        /// Thống kê tổng quan hệ thống
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetSystemStatistics()
        {
            try
            {
                var stats = new
                {
                    TotalUsers = await _context.Accounts.CountAsync(a => a.Role == Roles.User),
                    TotalCoaches = await _context.Accounts.CountAsync(a => a.Role == Roles.Coach),
                    TotalAdmins = await _context.Accounts.CountAsync(a => a.Role == Roles.Admin),
                    TotalPosts = await _context.CommunityPosts.CountAsync(),
                    TotalAchievements = await _context.Achievements.CountAsync(),
                    TotalQuitPlans = await _context.QuitPlans.CountAsync(),
                    TotalProgressRecords = await _context.ProgressRecords.CountAsync(),
                    NewUsersThisMonth = await _context.Accounts
                        .CountAsync(a => a.CreatedAt >= DateTime.UtcNow.AddMonths(-1))
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy thống kê", error = ex.Message });
            }
        }

        /// <summary>
        /// Thay đổi role của user
        /// </summary>
        [HttpPut("change-role/{accountId}")]
        public async Task<ActionResult> ChangeUserRole(int accountId, [FromBody] ChangeRoleDto changeRoleDto)
        {
            try
            {
                if (!Roles.IsValidRole(changeRoleDto.NewRole))
                {
                    return BadRequest(new { message = $"Role không hợp lệ. Các role hợp lệ: {string.Join(", ", Roles.AllRoles)}" });
                }

                var account = await _context.Accounts.FindAsync(accountId);
                if (account == null)
                {
                    return NotFound(new { message = "Không tìm thấy tài khoản" });
                }

                var oldRole = account.Role;
                account.Role = changeRoleDto.NewRole;
                account.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    message = $"Đã thay đổi role từ {oldRole} thành {changeRoleDto.NewRole}",
                    accountId = accountId,
                    oldRole = oldRole,
                    newRole = changeRoleDto.NewRole
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi thay đổi role", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa tài khoản (chỉ Admin)
        /// </summary>
        [HttpDelete("accounts/{accountId}")]
        public async Task<ActionResult> DeleteAccount(int accountId)
        {
            try
            {
                var account = await _context.Accounts.FindAsync(accountId);
                if (account == null)
                {
                    return NotFound(new { message = "Không tìm thấy tài khoản" });
                }

                // Không cho phép xóa tài khoản Admin khác
                if (account.Role == Roles.Admin)
                {
                    return Forbid("Không thể xóa tài khoản Admin");
                }

                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã xóa tài khoản thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi xóa tài khoản", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy logs hoạt động gần đây
        /// </summary>
        [HttpGet("recent-activities")]
        public async Task<ActionResult<object>> GetRecentActivities()
        {
            try
            {
                var recentPosts = await _context.CommunityPosts
                    .Include(p => p.Account)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(10)
                    .Select(p => new
                    {
                        Type = "Post",
                        Title = p.Title,
                        User = p.Account.Username,
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync();

                var recentAccounts = await _context.Accounts
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(10)
                    .Select(a => new
                    {
                        Type = "Registration",
                        Title = $"New {a.Role}: {a.Username}",
                        User = a.Username,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync();

                var activities = recentPosts.Concat(recentAccounts)
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(20)
                    .ToList();

                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy hoạt động gần đây", error = ex.Message });
            }
        }
    }

    /// <summary>
    /// DTO để thay đổi role
    /// </summary>
    public class ChangeRoleDto
    {
        public string NewRole { get; set; } = string.Empty;
    }
} 