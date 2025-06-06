using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories.Interfaces
{
    public interface IPlanRepository : IRepository<Plan>
    {
        Task<IEnumerable<Plan>> GetUserPlansAsync(int userId);
        Task<IEnumerable<Plan>> GetCoachPlansAsync(int coachId);
        Task<IEnumerable<Plan>> GetActivePlansAsync();
    }
} 