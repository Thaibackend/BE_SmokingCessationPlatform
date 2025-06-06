using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data.Repositories.Interfaces;

namespace SmokingQuitSupportAPI.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            try
            {
                Console.WriteLine($"Base Repository - Adding entity: {typeof(T).Name}");
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Base Repository - Entity saved successfully");
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Base Repository error: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
} 