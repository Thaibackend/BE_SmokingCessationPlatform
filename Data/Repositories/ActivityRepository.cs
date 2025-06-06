using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data.Repositories.Interfaces;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories
{
    public class ActivityRepository : Repository<Activity>, IActivityRepository
    {
        public ActivityRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Activity>> GetUserActivitiesAsync(int userId)
        {
            return await _context.Activities
                .Where(a => a.UserId == userId)
                .Include(a => a.User)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Activity>> GetActivitiesByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Activities
                .Where(a => a.UserId == userId && a.Date >= startDate && a.Date <= endDate)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }
    }
} 