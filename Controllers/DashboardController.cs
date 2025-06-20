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
    public class DashboardController : ControllerBase
    {
        private readonly ISmokingStatusService _smokingStatusService;
        private readonly IProgressService _progressService;
        private readonly IAchievementService _achievementService;
        private readonly IQuitPlanService _quitPlanService;

        public DashboardController(
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
        [HttpGet("overview")]
        public async Task<ActionResult<object>> GetDashboardOverview()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                // Lấy thông tin từ các service
                var smokingStatus = await _smokingStatusService.GetSmokingStatusAsync(accountId);
                var progressStats = await _progressService.GetProgressStatisticsAsync(accountId);
                var currentStreak = await _progressService.GetCurrentStreakAsync(accountId);
                var longestStreak = await _progressService.GetLongestStreakAsync(accountId);
                var activePlan = await _quitPlanService.GetActiveQuitPlanAsync(accountId);
                var planStats = await _quitPlanService.GetQuitPlanStatisticsAsync(accountId);
                var unlockedAchievements = await _achievementService.GetUnlockedAchievementsAsync(accountId);

                var overview = new
                {
                    // Thông tin tình trạng hút thuốc
                    SmokingStatus = smokingStatus,
                    
                    // Thống kê tiến trình
                    Progress = new
                    {
                        Statistics = progressStats,
                        CurrentStreak = currentStreak,
                        LongestStreak = longestStreak
                    },
                    
                    // Kế hoạch cai thuốc
                    QuitPlan = new
                    {
                        ActivePlan = activePlan,
                        Statistics = planStats
                    },
                    
                    // Huy hiệu thành tích
                    Achievements = new
                    {
                        UnlockedCount = unlockedAchievements.Count(),
                        RecentUnlocked = unlockedAchievements.OrderByDescending(a => a.UnlockedAt).Take(5)
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
        [HttpGet("weekly-report")]
        public async Task<ActionResult<object>> GetWeeklyReport()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var endDate = DateTime.UtcNow.Date;
                var startDate = endDate.AddDays(-6); // 7 ngày gần nhất

                var weeklyProgress = await _progressService.GetProgressByDateRangeAsync(accountId, startDate, endDate);
                
                var report = new
                {
                    Period = new { StartDate = startDate, EndDate = endDate },
                    Summary = new
                    {
                        TotalDays = 7,
                        SmokeFreedays = weeklyProgress.Count(p => p.IsSmokeFreeDay),
                        TotalCigarettesSmoked = weeklyProgress.Sum(p => p.CigarettesSmoked),
                        TotalMoneySaved = weeklyProgress.Sum(p => p.MoneySaved),
                        AverageStressLevel = weeklyProgress.Where(p => p.StressLevel.HasValue).Average(p => p.StressLevel.Value)
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
        /// Lấy báo cáo tiến trình theo tháng
        /// </summary>
        [HttpGet("monthly-report")]
        public async Task<ActionResult<object>> GetMonthlyReport()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var endDate = DateTime.UtcNow.Date;
                var startDate = endDate.AddDays(-29); // 30 ngày gần nhất

                var monthlyProgress = await _progressService.GetProgressByDateRangeAsync(accountId, startDate, endDate);
                
                var report = new
                {
                    Period = new { StartDate = startDate, EndDate = endDate },
                    Summary = new
                    {
                        TotalDays = 30,
                        SmokeFreedays = monthlyProgress.Count(p => p.IsSmokeFreeDay),
                        TotalCigarettesSmoked = monthlyProgress.Sum(p => p.CigarettesSmoked),
                        TotalMoneySaved = monthlyProgress.Sum(p => p.MoneySaved),
                        AverageStressLevel = monthlyProgress.Where(p => p.StressLevel.HasValue).Average(p => p.StressLevel.Value),
                        SuccessRate = monthlyProgress.Any() ? (double)monthlyProgress.Count(p => p.IsSmokeFreeDay) / monthlyProgress.Count() * 100 : 0
                    },
                    WeeklyBreakdown = monthlyProgress
                        .GroupBy(p => p.Date.AddDays(-(int)p.Date.DayOfWeek))
                        .Select(g => new
                        {
                            WeekStart = g.Key,
                            SmokeFreedays = g.Count(p => p.IsSmokeFreeDay),
                            TotalCigarettes = g.Sum(p => p.CigarettesSmoked),
                            MoneySaved = g.Sum(p => p.MoneySaved)
                        })
                        .OrderBy(w => w.WeekStart)
                };

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy báo cáo tháng", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quan hệ thống (cho admin)
        /// </summary>
        [HttpGet("system-statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetSystemStatistics()
        {
            try
            {
                var overallStats = await _smokingStatusService.GetOverallStatisticsAsync();
                var achievementLeaderboard = await _achievementService.GetAchievementLeaderboardAsync(10);
                var moneyLeaderboard = await _smokingStatusService.GetLeaderboardByMoneySavedAsync(10);
                var daysLeaderboard = await _smokingStatusService.GetLeaderboardBySmokeFreesDaysAsync(10);

                var systemStats = new
                {
                    OverallStatistics = overallStats,
                    Leaderboards = new
                    {
                        TopAchievers = achievementLeaderboard,
                        TopMoneySavers = moneyLeaderboard,
                        LongestSmokeFree = daysLeaderboard
                    },
                    GeneratedAt = DateTime.UtcNow
                };

                return Ok(systemStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê hệ thống", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy dữ liệu cho biểu đồ tiến trình
        /// </summary>
        [HttpGet("progress-chart")]
        public async Task<ActionResult<object>> GetProgressChartData([FromQuery] int days = 30)
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var endDate = DateTime.UtcNow.Date;
                var startDate = endDate.AddDays(-(days - 1));

                var progressData = await _progressService.GetProgressByDateRangeAsync(accountId, startDate, endDate);
                
                var chartData = new
                {
                    Labels = Enumerable.Range(0, days)
                        .Select(i => startDate.AddDays(i).ToString("dd/MM"))
                        .ToArray(),
                    
                    Datasets = new object[]
                    {
                        new
                        {
                            Label = "Số điếu thuốc hút",
                            Data = Enumerable.Range(0, days)
                                .Select(i => {
                                    var date = startDate.AddDays(i);
                                    var progress = progressData.FirstOrDefault(p => p.Date.Date == date);
                                    return (double)(progress?.CigarettesSmoked ?? 0);
                                })
                                .ToArray(),
                            BorderColor = "rgb(255, 99, 132)",
                            BackgroundColor = "rgba(255, 99, 132, 0.2)"
                        },
                        new
                        {
                            Label = "Tiền tiết kiệm (nghìn đồng)",
                            Data = Enumerable.Range(0, days)
                                .Select(i => {
                                    var date = startDate.AddDays(i);
                                    var progress = progressData.FirstOrDefault(p => p.Date.Date == date);
                                    return Math.Round((double)(progress?.MoneySaved ?? 0) / 1000, 1);
                                })
                                .ToArray(),
                            BorderColor = "rgb(54, 162, 235)",
                            BackgroundColor = "rgba(54, 162, 235, 0.2)"
                        }
                    }
                };

                return Ok(chartData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy dữ liệu biểu đồ", error = ex.Message });
            }
        }
    }
} 