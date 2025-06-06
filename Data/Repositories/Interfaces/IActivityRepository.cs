using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories.Interfaces
{
    public interface IActivityRepository : IRepository<Activity>
    {
        Task<IEnumerable<Activity>> GetUserActivitiesAsync(int userId);
        Task<IEnumerable<Activity>> GetActivitiesByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    }
} 