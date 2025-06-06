using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data.Repositories.Interfaces;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories
{
    public class PlanTemplateRepository : Repository<PlanTemplate>, IPlanTemplateRepository
    {
        public PlanTemplateRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PlanTemplate>> GetActiveTemplatesAsync()
        {
            return await _context.PlanTemplates
                .Where(pt => pt.IsActive)
                .OrderBy(pt => pt.Category)
                .ThenBy(pt => pt.DurationDays)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlanTemplate>> GetTemplatesByCategoryAsync(string category)
        {
            return await _context.PlanTemplates
                .Where(pt => pt.IsActive && pt.Category == category)
                .OrderBy(pt => pt.DurationDays)
                .ToListAsync();
        }
    }
} 