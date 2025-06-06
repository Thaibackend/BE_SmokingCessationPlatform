using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories.Interfaces
{
    public interface ICoachApplicationRepository : IRepository<CoachApplication>
    {
        Task<IEnumerable<CoachApplication>> GetPendingApplicationsAsync();
        Task<CoachApplication?> GetUserApplicationAsync(int userId);
    }
} 