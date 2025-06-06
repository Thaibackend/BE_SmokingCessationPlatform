using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data.Repositories.Interfaces;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetByUserIdAsync(int userId)
        {
            return await _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Coach)
                .Where(a => a.MemberId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByCoachIdAsync(int coachId)
        {
            return await _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Coach)
                .Where(a => a.CoachId == coachId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Coach)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);
        }

        public async Task<Appointment> AddAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task DeleteAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }
    }
} 