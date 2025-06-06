using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories.Interfaces
{
    public interface IPackageRepository : IRepository<Package>
    {
        Task<IEnumerable<Package>> GetActivePackagesAsync();
        Task<IEnumerable<Package>> GetPackagesByCoachAsync(int coachId);
    }
} 