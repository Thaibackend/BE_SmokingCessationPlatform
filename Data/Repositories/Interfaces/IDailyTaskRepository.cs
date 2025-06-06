using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories.Interfaces
{
    public interface IDailyTaskRepository : IRepository<DailyTask>
    {
        Task<IEnumerable<DailyTask>> GetUserTasksAsync(int userId);
        Task<IEnumerable<DailyTask>> GetTasksByDateAsync(int userId, DateTime date);
        Task<IEnumerable<DailyTask>> GetPendingTasksAsync(int userId);
    }
} 