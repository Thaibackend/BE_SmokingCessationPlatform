using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data.Repositories.Interfaces;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories
{
    public class PlanRepository : Repository<Plan>, IPlanRepository
    {
        public PlanRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Plan>> GetUserPlansAsync(int userId)
        {
            return await _context.Plans
                .Where(p => p.MemberId == userId)
                .Include(p => p.Package)
                .Include(p => p.Coach)
                .Include(p => p.Member)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Plan>> GetCoachPlansAsync(int coachId)
        {
            return await _context.Plans
                .Where(p => p.CoachId == coachId)
                .Include(p => p.Package)
                .Include(p => p.Member)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Plan>> GetActivePlansAsync()
        {
            return await _context.Plans
                .Where(p => p.Status == "Active")
                .Include(p => p.Package)
                .Include(p => p.Coach)
                .Include(p => p.Member)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
} 