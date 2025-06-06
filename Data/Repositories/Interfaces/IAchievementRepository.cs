using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories.Interfaces
{
    public interface IAchievementRepository : IRepository<Achievement>
    {
        Task<IEnumerable<Achievement>> GetActiveAchievementsAsync();
    }
} 