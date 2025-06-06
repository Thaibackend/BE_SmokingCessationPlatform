using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data.Repositories.Interfaces;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories
{
    public class CoachApplicationRepository : Repository<CoachApplication>, ICoachApplicationRepository
    {
        public CoachApplicationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CoachApplication>> GetPendingApplicationsAsync()
        {
            return await _context.CoachApplications
                .Include(ca => ca.User)
                .Where(ca => ca.Status == "Pending")
                .OrderBy(ca => ca.ApplicationDate)
                .ToListAsync();
        }

        public async Task<CoachApplication?> GetUserApplicationAsync(int userId)
        {
            return await _context.CoachApplications
                .Include(ca => ca.User)
                .Where(ca => ca.UserId == userId)
                .OrderByDescending(ca => ca.ApplicationDate)
                .FirstOrDefaultAsync();
        }
    }
} 