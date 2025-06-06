using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories.Interfaces
{
    public interface ISmokingStatusRepository : IRepository<SmokingStatus>
    {
        Task<SmokingStatus?> GetUserCurrentStatusAsync(int userId);
        Task<IEnumerable<SmokingStatus>> GetUserStatusHistoryAsync(int userId);
    }
} 