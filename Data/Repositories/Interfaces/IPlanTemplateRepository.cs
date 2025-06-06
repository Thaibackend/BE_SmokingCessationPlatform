using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories.Interfaces
{
    public interface IPlanTemplateRepository : IRepository<PlanTemplate>
    {
        Task<IEnumerable<PlanTemplate>> GetActiveTemplatesAsync();
        Task<IEnumerable<PlanTemplate>> GetTemplatesByCategoryAsync(string category);
    }
} 