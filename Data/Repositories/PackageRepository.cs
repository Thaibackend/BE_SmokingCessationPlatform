using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data.Repositories.Interfaces;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories
{
    public class PackageRepository : Repository<Package>, IPackageRepository
    {
        public PackageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Package>> GetActivePackagesAsync()
        {
            return await _context.Packages
                .Where(p => p.IsActive)
                .Include(p => p.AssignedCoach)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Package>> GetPackagesByCoachAsync(int coachId)
        {
            return await _context.Packages
                .Where(p => p.AssignedCoachId == coachId && p.IsActive)
                .Include(p => p.AssignedCoach)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Package> AddAsync(Package package)
        {
            try
            {
                Console.WriteLine($"Adding package to database: {package.Name}");
                _context.Packages.Add(package);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Package saved successfully with ID: {package.PackageId}");
                return package;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository error: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
} 