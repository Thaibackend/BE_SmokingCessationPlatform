using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Services.Interfaces;
using System.Security.Claims;

namespace SmokingQuitSupportAPI.Controllers
{
    /// <summary>
    /// Controller cho Dashboard & Report - Tổng quan và báo cáo
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly ISmokingStatusService _smokingStatusService;
        private readonly IProgressService _progressService;
        private readonly IAchievementService _achievementService;
        private readonly IQuitPlanService _quitPlanService;

        public ReportController(
            ISmokingStatusService smokingStatusService,
            IProgressService progressService,
            IAchievementService achievementService,
            IQuitPlanService quitPlanService)
        {
            _smokingStatusService = smokingStatusService;
            _progressService = progressService;
            _achievementService = achievementService;
            _quitPlanService = quitPlanService;
        }

        /// <summary>
        /// Lấy tổng quan dashboard cá nhân
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboardOverview()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                // Lấy thông tin từ các service
                var smokingStatus = await _smokingStatusService.GetSmokingStatusAsync(accountId);
                var progressStats = await _progressService.GetProgressStatisticsAsync(accountId);
                var currentStreak = await _progressService.GetCurrentStreakAsync(accountId);
                var activePlan = await _quitPlanService.GetActiveQuitPlanAsync(accountId);
                var unlockedAchievements = await _achievementService.GetUnlockedAchievementsAsync(accountId);

                var overview = new
                {
                    // Thông tin tình trạng hút thuốc
                    SmokingStatus = smokingStatus,
                    
                    // Thống kê tiến trình
                    Progress = new
                    {
                        Statistics = progressStats,
                        CurrentStreak = currentStreak
                    },
                    
                    // Kế hoạch cai thuốc
                    ActivePlan = activePlan,
                    
                    // Huy hiệu thành tích
                    Achievements = new
                    {
                        UnlockedCount = unlockedAchievements.Count(),
                        RecentUnlocked = unlockedAchievements.OrderByDescending(a => a.UnlockedAt).Take(3)
                    },
                    
                    // Thời gian cập nhật
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(overview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy tổng quan dashboard", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy báo cáo tiến trình theo tuần
        /// </summary>
        [HttpGet("weekly")]
        public async Task<ActionResult<object>> GetWeeklyReport()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var endDate = DateTime.UtcNow.Date;
                var startDate = endDate.AddDays(-6);

                var weeklyProgress = await _progressService.GetProgressByDateRangeAsync(accountId, startDate, endDate);
                
                var report = new
                {
                    Period = new { StartDate = startDate, EndDate = endDate },
                    Summary = new
                    {
                        TotalDays = 7,
                        SmokeFreedays = weeklyProgress.Count(p => p.IsSmokeFreeDay),
                        TotalCigarettesSmoked = weeklyProgress.Sum(p => p.CigarettesSmoked),
                        TotalMoneySaved = weeklyProgress.Sum(p => p.MoneySaved)
                    },
                    DailyData = weeklyProgress.OrderBy(p => p.Date)
                };

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy báo cáo tuần", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quan hệ thống
        /// </summary>
        [HttpGet("system-stats")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetSystemStatistics()
        {
            try
            {
                var overallStats = await _smokingStatusService.GetOverallStatisticsAsync();
                var moneyLeaderboard = await _smokingStatusService.GetLeaderboardByMoneySavedAsync(5);
                var daysLeaderboard = await _smokingStatusService.GetLeaderboardBySmokeFreesDaysAsync(5);

                var systemStats = new
                {
                    OverallStatistics = overallStats,
                    TopMoneySavers = moneyLeaderboard,
                    LongestSmokeFree = daysLeaderboard,
                    GeneratedAt = DateTime.UtcNow
                };

                return Ok(systemStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê hệ thống", error = ex.Message });
            }
        }
    }
} 