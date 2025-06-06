using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data.Repositories.Interfaces;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories
{
    public class SmokingStatusRepository : Repository<SmokingStatus>, ISmokingStatusRepository
    {
        public SmokingStatusRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<SmokingStatus?> GetUserCurrentStatusAsync(int userId)
        {
            return await _context.SmokingStatuses
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SmokingStatus>> GetUserStatusHistoryAsync(int userId)
        {
            return await _context.SmokingStatuses
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }
    }
} 