using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data.Repositories.Interfaces;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories
{
    public class DailyTaskRepository : Repository<DailyTask>, IDailyTaskRepository
    {
        public DailyTaskRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DailyTask>> GetUserTasksAsync(int userId)
        {
            return await _context.DailyTasks
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DailyTask>> GetTasksByDateAsync(int userId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return await _context.DailyTasks
                .Where(t => t.UserId == userId && t.DueDate >= startDate && t.DueDate < endDate)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DailyTask>> GetPendingTasksAsync(int userId)
        {
            return await _context.DailyTasks
                .Where(t => t.UserId == userId && t.Status == "Pending")
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }
    }
} 