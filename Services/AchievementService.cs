using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data;
using SmokingQuitSupportAPI.Models.DTOs.Achievement;
using SmokingQuitSupportAPI.Models.Entities;
using SmokingQuitSupportAPI.Services.Interfaces;

namespace SmokingQuitSupportAPI.Services
{
    /// <summary>
    /// Service xử lý logic huy hiệu thành tích
    /// </summary>
    public class AchievementService : IAchievementService
    {
        private readonly AppDbContext _context;

        public AchievementService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AchievementDto>> GetAllAchievementsAsync()
        {
            var achievements = await _context.Achievements
                .Select(a => new AchievementDto
                {
                    AchievementId = a.AchievementId,
                    Name = a.Name,
                    Description = a.Description,
                    Icon = a.Icon,
                    Type = a.Type,
                    RequiredValue = a.RequiredValue,
                    BadgeColor = a.BadgeColor,
                    CreatedAt = a.CreatedAt,
                    TotalUnlocks = a.UserAchievements.Count
                })
                .ToListAsync();

            return achievements;
        }

        public async Task<IEnumerable<AchievementDto>> GetUserAchievementsAsync(int accountId)
        {
            var achievements = await _context.Achievements
                .Select(a => new AchievementDto
                {
                    AchievementId = a.AchievementId,
                    Name = a.Name,
                    Description = a.Description,
                    Icon = a.Icon,
                    Type = a.Type,
                    RequiredValue = a.RequiredValue,
                    BadgeColor = a.BadgeColor,
                    CreatedAt = a.CreatedAt,
                    IsUnlocked = a.UserAchievements.Any(ua => ua.AccountId == accountId),
                    UnlockedAt = a.UserAchievements
                        .Where(ua => ua.AccountId == accountId)
                        .Select(ua => ua.UnlockedAt)
                        .FirstOrDefault(),
                    CurrentProgress = CalculateProgress(accountId, a.Type, a.RequiredValue),
                    TotalUnlocks = a.UserAchievements.Count
                })
                .ToListAsync();

            return achievements;
        }

        public async Task<IEnumerable<AchievementDto>> GetUnlockedAchievementsAsync(int accountId)
        {
            var unlockedAchievements = await _context.UserAchievements
                .Where(ua => ua.AccountId == accountId)
                .Include(ua => ua.Achievement)
                .Select(ua => new AchievementDto
                {
                    AchievementId = ua.Achievement.AchievementId,
                    Name = ua.Achievement.Name,
                    Description = ua.Achievement.Description,
                    Icon = ua.Achievement.Icon,
                    Type = ua.Achievement.Type,
                    RequiredValue = ua.Achievement.RequiredValue,
                    BadgeColor = ua.Achievement.BadgeColor,
                    CreatedAt = ua.Achievement.CreatedAt,
                    IsUnlocked = true,
                    UnlockedAt = ua.UnlockedAt,
                    CurrentProgress = ua.Achievement.RequiredValue,
                    TotalUnlocks = ua.Achievement.UserAchievements.Count
                })
                .ToListAsync();

            return unlockedAchievements;
        }

        public async Task<IEnumerable<AchievementDto>> CheckAndUnlockAchievementsAsync(int accountId)
        {
            var newlyUnlocked = new List<AchievementDto>();
            
            // Lấy các achievement chưa unlock
            var unlockedAchievementIds = await _context.UserAchievements
                .Where(ua => ua.AccountId == accountId)
                .Select(ua => ua.AchievementId)
                .ToListAsync();

            var availableAchievements = await _context.Achievements
                .Where(a => !unlockedAchievementIds.Contains(a.AchievementId))
                .ToListAsync();

            foreach (var achievement in availableAchievements)
            {
                var currentProgress = CalculateProgress(accountId, achievement.Type, achievement.RequiredValue);
                
                if (currentProgress >= achievement.RequiredValue)
                {
                    // Unlock achievement
                    await UnlockAchievementAsync(accountId, achievement.AchievementId);
                    
                    newlyUnlocked.Add(new AchievementDto
                    {
                        AchievementId = achievement.AchievementId,
                        Name = achievement.Name,
                        Description = achievement.Description,
                        Icon = achievement.Icon,
                        Type = achievement.Type,
                        RequiredValue = achievement.RequiredValue,
                        BadgeColor = achievement.BadgeColor,
                        CreatedAt = achievement.CreatedAt,
                        IsUnlocked = true,
                        UnlockedAt = DateTime.UtcNow,
                        CurrentProgress = currentProgress
                    });
                }
            }

            return newlyUnlocked;
        }

        public async Task<bool> UnlockAchievementAsync(int accountId, int achievementId)
        {
            try
            {
                // Kiểm tra xem achievement đã được unlock chưa
                var existingUnlock = await _context.UserAchievements
                    .FirstOrDefaultAsync(ua => ua.AccountId == accountId && ua.AchievementId == achievementId);

                if (existingUnlock != null)
                    return false; // Đã unlock rồi

                var userAchievement = new UserAchievement
                {
                    AccountId = accountId,
                    AchievementId = achievementId,
                    UnlockedAt = DateTime.UtcNow
                };

                _context.UserAchievements.Add(userAchievement);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<AchievementDto?> GetAchievementProgressAsync(int accountId, int achievementId)
        {
            var achievement = await _context.Achievements
                .FirstOrDefaultAsync(a => a.AchievementId == achievementId);

            if (achievement == null)
                return null;

            var userAchievement = await _context.UserAchievements
                .FirstOrDefaultAsync(ua => ua.AccountId == accountId && ua.AchievementId == achievementId);

            var currentProgress = CalculateProgress(accountId, achievement.Type, achievement.RequiredValue);

            return new AchievementDto
            {
                AchievementId = achievement.AchievementId,
                Name = achievement.Name,
                Description = achievement.Description,
                Icon = achievement.Icon,
                Type = achievement.Type,
                RequiredValue = achievement.RequiredValue,
                BadgeColor = achievement.BadgeColor,
                CreatedAt = achievement.CreatedAt,
                IsUnlocked = userAchievement != null,
                UnlockedAt = userAchievement?.UnlockedAt,
                CurrentProgress = currentProgress,
                TotalUnlocks = achievement.UserAchievements?.Count ?? 0
            };
        }

        public async Task<IEnumerable<object>> GetAchievementLeaderboardAsync(int take = 10)
        {
            var leaderboard = await _context.Accounts
                .Select(a => new
                {
                    AccountId = a.AccountId,
                    Username = a.Username,
                    AchievementCount = a.UserAchievements.Count,
                    LatestAchievement = a.UserAchievements
                        .OrderByDescending(ua => ua.UnlockedAt)
                        .Select(ua => ua.Achievement.Name)
                        .FirstOrDefault()
                })
                .Where(x => x.AchievementCount > 0)
                .OrderByDescending(x => x.AchievementCount)
                .Take(take)
                .ToListAsync();

            return leaderboard;
        }

        public async Task<bool> ShareAchievementAsync(int accountId, int achievementId)
        {
            try
            {
                // Kiểm tra xem user có achievement này không
                var userAchievement = await _context.UserAchievements
                    .Include(ua => ua.Achievement)
                    .FirstOrDefaultAsync(ua => ua.AccountId == accountId && ua.AchievementId == achievementId);

                if (userAchievement == null)
                    return false;

                // Tạo notification hoặc post (tùy yêu cầu)
                // Ở đây tôi sẽ tạo một notification đơn giản
                var notification = new Notification
                {
                    AccountId = accountId,
                    Title = "Achievement Shared",
                    Message = $"You shared your achievement: {userAchievement.Achievement.Name}",
                    Type = "ACHIEVEMENT_SHARE",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tính toán tiến trình hiện tại của user cho một loại achievement
        /// </summary>
        private int CalculateProgress(int accountId, string achievementType, int requiredValue)
        {
            // Tính toán dựa trên type của achievement
            return achievementType.ToUpper() switch
            {
                "SMOKE_FREE_DAYS" => GetSmokeFreenDays(accountId),
                "MONEY_SAVED" => GetMoneySaved(accountId),
                "POSTS_CREATED" => GetPostsCreated(accountId),
                "COMMENTS_MADE" => GetCommentsMade(accountId),
                "PLANS_COMPLETED" => GetPlansCompleted(accountId),
                _ => 0
            };
        }

        private int GetSmokeFreenDays(int accountId)
        {
            var smokingStatus = _context.SmokingStatuses
                .Where(s => s.AccountId == accountId)
                .OrderByDescending(s => s.StatusId)
                .FirstOrDefault();

            return smokingStatus?.SmokeFreenDays ?? 0;
        }

        private int GetMoneySaved(int accountId)
        {
            var smokingStatus = _context.SmokingStatuses
                .Where(s => s.AccountId == accountId)
                .OrderByDescending(s => s.StatusId)
                .FirstOrDefault();

            return (int)(smokingStatus?.MoneySaved ?? 0);
        }

        private int GetPostsCreated(int accountId)
        {
            return _context.CommunityPosts
                .Count(p => p.AccountId == accountId);
        }

        private int GetCommentsMade(int accountId)
        {
            return _context.Comments
                .Count(c => c.AccountId == accountId);
        }

        private int GetPlansCompleted(int accountId)
        {
            return _context.QuitPlans
                .Count(q => q.MemberId == accountId && q.Status == "COMPLETED");
        }
    }
} 