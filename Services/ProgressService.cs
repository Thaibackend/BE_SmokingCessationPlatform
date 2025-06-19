using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data;
using SmokingQuitSupportAPI.Models.DTOs.Progress;
using SmokingQuitSupportAPI.Models.Entities;
using SmokingQuitSupportAPI.Services.Interfaces;

namespace SmokingQuitSupportAPI.Services
{
    /// <summary>
    /// Service xử lý logic tiến trình cai thuốc
    /// </summary>
    public class ProgressService : IProgressService
    {
        private readonly AppDbContext _context;

        public ProgressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProgressDto>> GetUserProgressAsync(int accountId)
        {
            var progress = await _context.ProgressRecords
                .Where(p => p.AccountId == accountId)
                .OrderByDescending(p => p.Date)
                .Select(p => new ProgressDto
                {
                    ProgressId = p.ProgressId,
                    AccountId = p.AccountId,
                    Date = p.Date,
                    SmokeFreenDays = p.SmokeFreenDays,
                    CigarettesAvoided = p.CigarettesAvoided,
                    MoneySaved = p.MoneySaved,
                    HealthScore = p.HealthScore,
                    Notes = p.Notes,
                    Mood = p.Mood,
                    CravingLevel = p.CravingLevel,
                    Weight = p.Weight,
                    ExerciseMinutes = p.ExerciseMinutes,
                    SleepHours = p.SleepHours
                })
                .ToListAsync();

            return progress;
        }

        public async Task<ProgressDto?> GetProgressByDateAsync(int accountId, DateTime date)
        {
            var dateOnly = date.Date;
            
            var progress = await _context.ProgressRecords
                .Where(p => p.AccountId == accountId && p.Date.Date == dateOnly)
                .Select(p => new ProgressDto
                {
                    ProgressId = p.ProgressId,
                    AccountId = p.AccountId,
                    Date = p.Date,
                    SmokeFreenDays = p.SmokeFreenDays,
                    CigarettesAvoided = p.CigarettesAvoided,
                    MoneySaved = p.MoneySaved,
                    HealthScore = p.HealthScore,
                    Notes = p.Notes,
                    Mood = p.Mood,
                    CravingLevel = p.CravingLevel,
                    Weight = p.Weight,
                    ExerciseMinutes = p.ExerciseMinutes,
                    SleepHours = p.SleepHours
                })
                .FirstOrDefaultAsync();

            return progress;
        }

        public async Task<IEnumerable<ProgressDto>> GetProgressByDateRangeAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            var startDateOnly = startDate.Date;
            var endDateOnly = endDate.Date;

            var progress = await _context.ProgressRecords
                .Where(p => p.AccountId == accountId && 
                           p.Date.Date >= startDateOnly && 
                           p.Date.Date <= endDateOnly)
                .OrderBy(p => p.Date)
                .Select(p => new ProgressDto
                {
                    ProgressId = p.ProgressId,
                    AccountId = p.AccountId,
                    Date = p.Date,
                    SmokeFreenDays = p.SmokeFreenDays,
                    CigarettesAvoided = p.CigarettesAvoided,
                    MoneySaved = p.MoneySaved,
                    HealthScore = p.HealthScore,
                    Notes = p.Notes,
                    Mood = p.Mood,
                    CravingLevel = p.CravingLevel,
                    Weight = p.Weight,
                    ExerciseMinutes = p.ExerciseMinutes,
                    SleepHours = p.SleepHours
                })
                .ToListAsync();

            return progress;
        }

        public async Task<ProgressDto> RecordDailyProgressAsync(int accountId, CreateProgressDto createProgressDto)
        {
            // Kiểm tra xem đã có record cho ngày này chưa
            var existingProgress = await _context.ProgressRecords
                .FirstOrDefaultAsync(p => p.AccountId == accountId && p.Date.Date == createProgressDto.Date.Date);

            if (existingProgress != null)
            {
                throw new InvalidOperationException("Progress for this date already exists. Use update instead.");
            }

            // Tính toán smokeFreenDays dựa trên quit date
            var smokingStatus = await _context.SmokingStatuses
                .Where(s => s.AccountId == accountId)
                .OrderByDescending(s => s.StatusId)
                .FirstOrDefaultAsync();

            var quitDate = smokingStatus?.QuitDate ?? DateTime.UtcNow.Date;
            var smokeFreenDays = (createProgressDto.Date.Date - quitDate.Date).Days + 1;

            var progress = new Progress
            {
                AccountId = accountId,
                Date = createProgressDto.Date,
                SmokeFreenDays = Math.Max(0, smokeFreenDays),
                CigarettesAvoided = createProgressDto.CigarettesAvoided,
                MoneySaved = createProgressDto.MoneySaved,
                HealthScore = createProgressDto.HealthScore,
                Notes = createProgressDto.Notes,
                Mood = createProgressDto.Mood,
                CravingLevel = createProgressDto.CravingLevel,
                Weight = createProgressDto.Weight,
                ExerciseMinutes = createProgressDto.ExerciseMinutes,
                SleepHours = createProgressDto.SleepHours
            };

            _context.ProgressRecords.Add(progress);
            await _context.SaveChangesAsync();

            // Update automatic statistics
            await UpdateAutomaticStatisticsAsync(accountId);

            return new ProgressDto
            {
                ProgressId = progress.ProgressId,
                AccountId = progress.AccountId,
                Date = progress.Date,
                SmokeFreenDays = progress.SmokeFreenDays,
                CigarettesAvoided = progress.CigarettesAvoided,
                MoneySaved = progress.MoneySaved,
                HealthScore = progress.HealthScore,
                Notes = progress.Notes,
                Mood = progress.Mood,
                CravingLevel = progress.CravingLevel,
                Weight = progress.Weight,
                ExerciseMinutes = progress.ExerciseMinutes,
                SleepHours = progress.SleepHours
            };
        }

        public async Task<ProgressDto?> UpdateProgressAsync(int progressId, int accountId, CreateProgressDto updateProgressDto)
        {
            var progress = await _context.ProgressRecords
                .FirstOrDefaultAsync(p => p.ProgressId == progressId && p.AccountId == accountId);

            if (progress == null)
                return null;

            progress.CigarettesAvoided = updateProgressDto.CigarettesAvoided;
            progress.MoneySaved = updateProgressDto.MoneySaved;
            progress.HealthScore = updateProgressDto.HealthScore;
            progress.Notes = updateProgressDto.Notes;
            progress.Mood = updateProgressDto.Mood;
            progress.CravingLevel = updateProgressDto.CravingLevel;
            progress.Weight = updateProgressDto.Weight;
            progress.ExerciseMinutes = updateProgressDto.ExerciseMinutes;
            progress.SleepHours = updateProgressDto.SleepHours;

            await _context.SaveChangesAsync();

            // Update automatic statistics
            await UpdateAutomaticStatisticsAsync(accountId);

            return new ProgressDto
            {
                ProgressId = progress.ProgressId,
                AccountId = progress.AccountId,
                Date = progress.Date,
                SmokeFreenDays = progress.SmokeFreenDays,
                CigarettesAvoided = progress.CigarettesAvoided,
                MoneySaved = progress.MoneySaved,
                HealthScore = progress.HealthScore,
                Notes = progress.Notes,
                Mood = progress.Mood,
                CravingLevel = progress.CravingLevel,
                Weight = progress.Weight,
                ExerciseMinutes = progress.ExerciseMinutes,
                SleepHours = progress.SleepHours
            };
        }

        public async Task<bool> DeleteProgressAsync(int progressId, int accountId)
        {
            var progress = await _context.ProgressRecords
                .FirstOrDefaultAsync(p => p.ProgressId == progressId && p.AccountId == accountId);

            if (progress == null)
                return false;

            _context.ProgressRecords.Remove(progress);
            await _context.SaveChangesAsync();

            // Update automatic statistics
            await UpdateAutomaticStatisticsAsync(accountId);

            return true;
        }

        public async Task<object> GetProgressStatisticsAsync(int accountId)
        {
            var progressRecords = await _context.ProgressRecords
                .Where(p => p.AccountId == accountId)
                .ToListAsync();

            if (!progressRecords.Any())
            {
                return new
                {
                    TotalDays = 0,
                    CurrentStreak = 0,
                    LongestStreak = 0,
                    TotalCigarettesAvoided = 0,
                    TotalMoneySaved = 0m,
                    AverageHealthScore = 0.0,
                    AverageMood = 0.0,
                    AverageCravingLevel = 0.0,
                    TotalExerciseMinutes = 0,
                    AverageSleepHours = 0.0
                };
            }

            var currentStreak = await GetCurrentStreakAsync(accountId);
            var longestStreak = await GetLongestStreakAsync(accountId);

            return new
            {
                TotalDays = progressRecords.Count,
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                TotalCigarettesAvoided = progressRecords.Sum(p => p.CigarettesAvoided),
                TotalMoneySaved = progressRecords.Sum(p => p.MoneySaved),
                AverageHealthScore = progressRecords.Average(p => p.HealthScore ?? 0),
                AverageMood = progressRecords.Where(p => p.Mood.HasValue).Average(p => p.Mood.Value),
                AverageCravingLevel = progressRecords.Where(p => p.CravingLevel.HasValue).Average(p => p.CravingLevel.Value),
                TotalExerciseMinutes = progressRecords.Sum(p => p.ExerciseMinutes ?? 0),
                AverageSleepHours = progressRecords.Where(p => p.SleepHours.HasValue).Average(p => p.SleepHours.Value),
                LastUpdated = progressRecords.Max(p => p.Date)
            };
        }

        public async Task<int> GetCurrentStreakAsync(int accountId)
        {
            var progressRecords = await _context.ProgressRecords
                .Where(p => p.AccountId == accountId)
                .OrderByDescending(p => p.Date)
                .ToListAsync();

            if (!progressRecords.Any())
                return 0;

            int streak = 0;
            var currentDate = DateTime.UtcNow.Date;

            foreach (var progress in progressRecords)
            {
                if (progress.Date.Date == currentDate.AddDays(-streak))
                {
                    streak++;
                }
                else
                {
                    break;
                }
            }

            return streak;
        }

        public async Task<int> GetLongestStreakAsync(int accountId)
        {
            var progressRecords = await _context.ProgressRecords
                .Where(p => p.AccountId == accountId)
                .OrderBy(p => p.Date)
                .ToListAsync();

            if (!progressRecords.Any())
                return 0;

            int longestStreak = 1;
            int currentStreak = 1;

            for (int i = 1; i < progressRecords.Count; i++)
            {
                var currentDate = progressRecords[i].Date.Date;
                var previousDate = progressRecords[i - 1].Date.Date;

                if ((currentDate - previousDate).Days == 1)
                {
                    currentStreak++;
                    longestStreak = Math.Max(longestStreak, currentStreak);
                }
                else
                {
                    currentStreak = 1;
                }
            }

            return longestStreak;
        }

        public async Task UpdateAutomaticStatisticsAsync(int accountId)
        {
            // Cập nhật SmokingStatus với thống kê mới nhất
            var smokingStatus = await _context.SmokingStatuses
                .Where(s => s.AccountId == accountId)
                .OrderByDescending(s => s.StatusId)
                .FirstOrDefaultAsync();

            if (smokingStatus == null)
                return;

            var statistics = await GetProgressStatisticsAsync(accountId);
            dynamic stats = statistics;

            smokingStatus.SmokeFreenDays = await GetCurrentStreakAsync(accountId);
            smokingStatus.MoneySaved = stats.TotalMoneySaved;
            smokingStatus.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
} 