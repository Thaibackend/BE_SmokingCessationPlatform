using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Attributes;
using SmokingQuitSupportAPI.Data;
using SmokingQuitSupportAPI.Models.DTOs.QuitPlan;
using SmokingQuitSupportAPI.Models.Entities;
using SmokingQuitSupportAPI.Constants;
using System.Security.Claims;

namespace SmokingQuitSupportAPI.Controllers
{
    /// <summary>
    /// Controller dành cho Coach quản lý coaching
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AdminOrCoachRequired]
    public class CoachController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoachController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách clients của coach
        /// </summary>
        [HttpGet("my-clients")]
        public async Task<ActionResult<IEnumerable<object>>> GetMyClients()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var coach = await _context.Coaches
                    .FirstOrDefaultAsync(c => c.AccountId == currentUserId);

                if (coach == null)
                {
                    return NotFound(new { message = "Coach profile không tồn tại" });
                }

                var clients = await _context.QuitPlans
                    .Where(qp => qp.CoachId == coach.CoachId)
                    .Include(qp => qp.Member)
                    .Select(qp => new
                    {
                        ClientId = qp.Member.AccountId,
                        ClientName = qp.Member.FullName ?? qp.Member.Username,
                        Email = qp.Member.Email,
                        QuitPlanTitle = qp.Name,
                        QuitPlanId = qp.PlanId,
                        Status = qp.Status,
                        CreatedAt = qp.CreatedAt
                    })
                    .ToListAsync();

                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách clients", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê coaching của coach
        /// </summary>
        [HttpGet("my-statistics")]
        public async Task<ActionResult<object>> GetMyCoachingStatistics()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var coach = await _context.Coaches
                    .FirstOrDefaultAsync(c => c.AccountId == currentUserId);

                if (coach == null)
                {
                    return NotFound(new { message = "Coach profile không tồn tại" });
                }

                var stats = new
                {
                    TotalClients = await _context.QuitPlans.CountAsync(qp => qp.CoachId == coach.CoachId),
                    ActivePlans = await _context.QuitPlans.CountAsync(qp => qp.CoachId == coach.CoachId && qp.Status == "ACTIVE"),
                    CompletedPlans = await _context.QuitPlans.CountAsync(qp => qp.CoachId == coach.CoachId && qp.Status == "COMPLETED"),
                    TotalSessions = coach.SessionsCompleted,
                    Rating = coach.Rating,
                    NewClientsThisMonth = await _context.QuitPlans
                        .CountAsync(qp => qp.CoachId == coach.CoachId && qp.CreatedAt >= DateTime.UtcNow.AddMonths(-1))
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy thống kê", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo quit plan cho client
        /// </summary>
        [HttpPost("create-plan-for-client")]
        public async Task<ActionResult<QuitPlanDto>> CreateQuitPlanForClient([FromBody] CreateQuitPlanForClientDto createDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var coach = await _context.Coaches
                    .FirstOrDefaultAsync(c => c.AccountId == currentUserId);

                if (coach == null)
                {
                    return NotFound(new { message = "Coach profile không tồn tại" });
                }

                // Kiểm tra client tồn tại
                var client = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountId == createDto.ClientId && a.Role == Roles.User);

                if (client == null)
                {
                    return NotFound(new { message = "Client không tồn tại" });
                }

                var quitPlan = new QuitPlan
                {
                    MemberId = createDto.ClientId,
                    CoachId = coach.CoachId,
                    Name = createDto.Title,
                    Description = createDto.Description,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(createDto.TargetDays),
                    PackageId = createDto.PackageId,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.QuitPlans.Add(quitPlan);
                await _context.SaveChangesAsync();

                var quitPlanDto = new QuitPlanDto
                {
                    PlanId = quitPlan.PlanId,
                    MemberId = quitPlan.MemberId,
                    CoachId = quitPlan.CoachId,
                    Name = quitPlan.Name,
                    Description = quitPlan.Description,
                    StartDate = quitPlan.StartDate,
                    EndDate = quitPlan.EndDate,
                    PackageId = quitPlan.PackageId,
                    Status = quitPlan.Status,
                    CreatedAt = quitPlan.CreatedAt,
                    UpdatedAt = quitPlan.UpdatedAt,
                    CoachName = coach.Account.Username
                };

                return CreatedAtAction(nameof(GetMyClients), new { id = quitPlan.PlanId }, quitPlanDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo quit plan", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật progress cho client
        /// </summary>
        [HttpPost("update-client-progress")]
        public async Task<ActionResult> UpdateClientProgress([FromBody] UpdateClientProgressDto updateDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var coach = await _context.Coaches
                    .FirstOrDefaultAsync(c => c.AccountId == currentUserId);

                if (coach == null)
                {
                    return NotFound(new { message = "Coach profile không tồn tại" });
                }

                // Kiểm tra client thuộc về coach này
                var quitPlan = await _context.QuitPlans
                    .FirstOrDefaultAsync(qp => qp.MemberId == updateDto.ClientId && qp.CoachId == coach.CoachId);

                if (quitPlan == null)
                {
                    return Forbid("Không có quyền truy cập client này");
                }

                // Tạo progress note
                var progress = new Progress
                {
                    AccountId = updateDto.ClientId,
                    Date = DateTime.UtcNow.Date,
                    Mood = updateDto.Mood,
                    CravingLevel = updateDto.CravingLevel,
                    Weight = updateDto.Weight,
                    ExerciseMinutes = updateDto.ExerciseMinutes,
                    SleepHours = updateDto.SleepHours,
                    Notes = $"[Coach Note] {updateDto.CoachNotes}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.ProgressRecords.Add(progress);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã cập nhật progress cho client thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi cập nhật progress", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả coaches (chỉ Admin)
        /// </summary>
        [HttpGet("all-coaches")]
        [AdminRequired]
        public async Task<ActionResult<IEnumerable<object>>> GetAllCoaches()
        {
            try
            {
                var coaches = await _context.Coaches
                    .Include(c => c.Account)
                    .Select(c => new
                    {
                        CoachId = c.CoachId,
                        Name = c.Account.FullName ?? c.Account.Username,
                        Email = c.Account.Email,
                        Qualifications = c.Qualifications,
                        Experience = c.Experience,
                        Bio = c.Bio,
                        Status = c.Status,
                        Rating = c.Rating,
                        SessionsCompleted = c.SessionsCompleted,
                        TotalClients = _context.QuitPlans.Count(qp => qp.CoachId == c.CoachId),
                        CreatedAt = c.CreatedAt
                    })
                    .ToListAsync();

                return Ok(coaches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách coaches", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy current user ID từ JWT token
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }
    }

    /// <summary>
    /// DTO để tạo quit plan cho client
    /// </summary>
    public class CreateQuitPlanForClientDto
    {
        public int ClientId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TargetDays { get; set; }
        public int PackageId { get; set; } = 1; // Default package
    }

    /// <summary>
    /// DTO để cập nhật progress cho client
    /// </summary>
    public class UpdateClientProgressDto
    {
        public int ClientId { get; set; }
        public int Mood { get; set; }
        public int CravingLevel { get; set; }
        public decimal? Weight { get; set; }
        public int? ExerciseMinutes { get; set; }
        public decimal? SleepHours { get; set; }
        public string CoachNotes { get; set; } = string.Empty;
    }
} 