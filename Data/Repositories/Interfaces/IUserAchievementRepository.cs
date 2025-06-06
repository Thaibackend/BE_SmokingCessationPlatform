using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories.Interfaces
{
    public interface IUserAchievementRepository : IRepository<UserAchievement>
    {
        Task<IEnumerable<UserAchievement>> GetUserAchievementsAsync(int userId);
        Task<bool> HasUserAchievementAsync(int userId, int achievementId);
    }
} 