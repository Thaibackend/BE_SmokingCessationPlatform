using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data.Repositories.Interfaces;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories
{
    public class UserAchievementRepository : Repository<UserAchievement>, IUserAchievementRepository
    {
        public UserAchievementRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserAchievement>> GetUserAchievementsAsync(int userId)
        {
            return await _context.UserAchievements
                .Include(ua => ua.Achievement)
                .Include(ua => ua.User)
                .Where(ua => ua.UserId == userId)
                .OrderByDescending(ua => ua.UnlockedAt)
                .ToListAsync();
        }

        public async Task<bool> HasUserAchievementAsync(int userId, int achievementId)
        {
            return await _context.UserAchievements
                .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);
        }
    }
} 