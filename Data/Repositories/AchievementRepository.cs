using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data.Repositories.Interfaces;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories
{
    public class AchievementRepository : Repository<Achievement>, IAchievementRepository
    {
        public AchievementRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Achievement>> GetActiveAchievementsAsync()
        {
            return await _context.Achievements
                .Where(a => a.IsActive)
                .OrderBy(a => a.Points)
                .ToListAsync();
        }
    }
} 