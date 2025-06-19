using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data;
using SmokingQuitSupportAPI.Models.DTOs.QuitPlan;
using SmokingQuitSupportAPI.Models.Entities;
using SmokingQuitSupportAPI.Services.Interfaces;

namespace SmokingQuitSupportAPI.Services
{
    /// <summary>
    /// Service xử lý logic kế hoạch cai thuốc
    /// </summary>
    public class QuitPlanService : IQuitPlanService
    {
        private readonly AppDbContext _context;

        public QuitPlanService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuitPlanDto>> GetUserQuitPlansAsync(int accountId)
        {
            var quitPlans = await _context.QuitPlans
                .Where(q => q.MemberId == accountId)
                .Include(q => q.Package)
                .Include(q => q.Member)
                .Include(q => q.Coach)
                .ThenInclude(c => c!.Account)
                .OrderByDescending(q => q.CreatedAt)
                .Select(q => new QuitPlanDto
                {
                    PlanId = q.PlanId,
                    Name = q.Name,
                    Description = q.Description,
                    Status = q.Status,
                    StartDate = q.StartDate,
                    EndDate = q.EndDate,
                    PackageId = q.PackageId,
                    MemberId = q.MemberId,
                    CoachId = q.CoachId,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                    PackageName = q.Package.Name,
                    MemberName = q.Member.Username,
                    CoachName = q.Coach != null ? q.Coach.Account.Username : null
                })
                .ToListAsync();

            return quitPlans;
        }

        public async Task<QuitPlanDto?> GetQuitPlanByIdAsync(int planId)
        {
            var quitPlan = await _context.QuitPlans
                .Include(q => q.Package)
                .Include(q => q.Member)
                .Include(q => q.Coach)
                .ThenInclude(c => c!.Account)
                .Where(q => q.PlanId == planId)
                .Select(q => new QuitPlanDto
                {
                    PlanId = q.PlanId,
                    Name = q.Name,
                    Description = q.Description,
                    Status = q.Status,
                    StartDate = q.StartDate,
                    EndDate = q.EndDate,
                    PackageId = q.PackageId,
                    MemberId = q.MemberId,
                    CoachId = q.CoachId,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                    PackageName = q.Package.Name,
                    MemberName = q.Member.Username,
                    CoachName = q.Coach != null ? q.Coach.Account.Username : null
                })
                .FirstOrDefaultAsync();

            return quitPlan;
        }

        public async Task<QuitPlanDto?> GetActiveQuitPlanAsync(int accountId)
        {
            var activePlan = await _context.QuitPlans
                .Where(q => q.MemberId == accountId && q.Status == "ACTIVE")
                .Include(q => q.Package)
                .Include(q => q.Member)
                .Include(q => q.Coach)
                .ThenInclude(c => c!.Account)
                .OrderByDescending(q => q.CreatedAt)
                .Select(q => new QuitPlanDto
                {
                    PlanId = q.PlanId,
                    Name = q.Name,
                    Description = q.Description,
                    Status = q.Status,
                    StartDate = q.StartDate,
                    EndDate = q.EndDate,
                    PackageId = q.PackageId,
                    MemberId = q.MemberId,
                    CoachId = q.CoachId,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                    PackageName = q.Package.Name,
                    MemberName = q.Member.Username,
                    CoachName = q.Coach != null ? q.Coach.Account.Username : null
                })
                .FirstOrDefaultAsync();

            return activePlan;
        }

        public async Task<QuitPlanDto> CreateQuitPlanAsync(int accountId, CreateQuitPlanDto createPlanDto)
        {
            // Verify that the package exists
            var package = await _context.MemberPackages
                .FirstOrDefaultAsync(p => p.PackageId == createPlanDto.PackageId && p.IsActive);

            if (package == null)
                throw new InvalidOperationException("Invalid or inactive package.");

            // Check if coach exists (if provided)
            if (createPlanDto.CoachId.HasValue)
            {
                var coach = await _context.Coaches
                    .FirstOrDefaultAsync(c => c.CoachId == createPlanDto.CoachId.Value && c.Status == "ACTIVE");

                if (coach == null)
                    throw new InvalidOperationException("Invalid or inactive coach.");
            }

            var quitPlan = new QuitPlan
            {
                Name = createPlanDto.Name,
                Description = createPlanDto.Description,
                Status = "ACTIVE",
                StartDate = createPlanDto.StartDate,
                EndDate = createPlanDto.EndDate,
                PackageId = createPlanDto.PackageId,
                MemberId = accountId,
                CoachId = createPlanDto.CoachId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.QuitPlans.Add(quitPlan);
            await _context.SaveChangesAsync();

            // Create initial smoking status if not exists
            await CreateInitialSmokingStatusAsync(accountId, createPlanDto.StartDate, createPlanDto.QuitReason);

            // Get the created plan with related data
            var createdPlan = await GetQuitPlanByIdAsync(quitPlan.PlanId);
            return createdPlan!;
        }

        public async Task<QuitPlanDto?> UpdateQuitPlanAsync(int planId, int accountId, CreateQuitPlanDto updatePlanDto)
        {
            var quitPlan = await _context.QuitPlans
                .FirstOrDefaultAsync(q => q.PlanId == planId && q.MemberId == accountId);

            if (quitPlan == null)
                return null;

            // Verify that the package exists
            var package = await _context.MemberPackages
                .FirstOrDefaultAsync(p => p.PackageId == updatePlanDto.PackageId && p.IsActive);

            if (package == null)
                throw new InvalidOperationException("Invalid or inactive package.");

            // Check if coach exists (if provided)
            if (updatePlanDto.CoachId.HasValue)
            {
                var coach = await _context.Coaches
                    .FirstOrDefaultAsync(c => c.CoachId == updatePlanDto.CoachId.Value && c.Status == "ACTIVE");

                if (coach == null)
                    throw new InvalidOperationException("Invalid or inactive coach.");
            }

            quitPlan.Name = updatePlanDto.Name;
            quitPlan.Description = updatePlanDto.Description;
            quitPlan.StartDate = updatePlanDto.StartDate;
            quitPlan.EndDate = updatePlanDto.EndDate;
            quitPlan.PackageId = updatePlanDto.PackageId;
            quitPlan.CoachId = updatePlanDto.CoachId;
            quitPlan.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetQuitPlanByIdAsync(planId);
        }

        public async Task<bool> DeleteQuitPlanAsync(int planId, int accountId)
        {
            var quitPlan = await _context.QuitPlans
                .FirstOrDefaultAsync(q => q.PlanId == planId && q.MemberId == accountId);

            if (quitPlan == null)
                return false;

            _context.QuitPlans.Remove(quitPlan);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<QuitPlanDto> GenerateAutomaticQuitPlanAsync(int accountId, int packageId)
        {
            // Get user's account information
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found.");

            // Get package information
            var package = await _context.MemberPackages
                .FirstOrDefaultAsync(p => p.PackageId == packageId && p.IsActive);
            if (package == null)
                throw new InvalidOperationException("Package not found or inactive.");

            // Get user's smoking status to understand their habit
            var smokingStatus = await _context.SmokingStatuses
                .Where(s => s.AccountId == accountId)
                .OrderByDescending(s => s.StatusId)
                .FirstOrDefaultAsync();

            // Generate plan based on package duration
            var startDate = DateTime.UtcNow.Date;
            var endDate = package.DurationDays > 0 ? startDate.AddDays(package.DurationDays) : (DateTime?)null;

            // Generate plan name based on package
            var planName = $"Kế hoạch cai thuốc - {package.Name}";
            var description = GeneratePlanDescription(package, smokingStatus);

            var createPlanDto = new CreateQuitPlanDto
            {
                Name = planName,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                PackageId = packageId,
                CoachId = package.AssignedCoachId,
                QuitReason = "Tự động tạo dựa trên gói thành viên"
            };

            return await CreateQuitPlanAsync(accountId, createPlanDto);
        }

        public async Task<bool> UpdatePlanStatusAsync(int planId, string status)
        {
            var quitPlan = await _context.QuitPlans.FirstOrDefaultAsync(q => q.PlanId == planId);
            if (quitPlan == null)
                return false;

            quitPlan.Status = status;
            quitPlan.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<object> GetQuitPlanStatisticsAsync(int accountId)
        {
            var plans = await _context.QuitPlans
                .Where(q => q.MemberId == accountId)
                .ToListAsync();

            if (!plans.Any())
            {
                return new
                {
                    TotalPlans = 0,
                    ActivePlans = 0,
                    CompletedPlans = 0,
                    FailedPlans = 0,
                    TotalDaysPlanned = 0,
                    AverageCompletionRate = 0.0,
                    CurrentPlanName = (string?)null,
                    CurrentPlanProgress = 0.0
                };
            }

            var activePlans = plans.Count(p => p.Status == "ACTIVE");
            var completedPlans = plans.Count(p => p.Status == "COMPLETED");
            var failedPlans = plans.Count(p => p.Status == "FAILED");

            var totalDaysPlanned = plans
                .Where(p => p.EndDate.HasValue)
                .Sum(p => (p.EndDate!.Value - p.StartDate).Days);

            var plansWithEndDate = plans.Where(p => p.EndDate.HasValue).ToList();
            var averageCompletionRate = plansWithEndDate.Any() 
                ? plansWithEndDate.Average(p => Math.Min(100, (DateTime.UtcNow - p.StartDate).Days / (double)(p.EndDate!.Value - p.StartDate).Days * 100))
                : 0.0;

            var currentPlan = plans.FirstOrDefault(p => p.Status == "ACTIVE");
            var currentPlanProgress = 0.0;
            
            if (currentPlan?.EndDate.HasValue == true)
            {
                var totalDays = (currentPlan.EndDate.Value - currentPlan.StartDate).Days;
                var completedDays = (DateTime.UtcNow - currentPlan.StartDate).Days;
                currentPlanProgress = totalDays > 0 ? Math.Min(100, (double)completedDays / totalDays * 100) : 0;
            }

            return new
            {
                TotalPlans = plans.Count,
                ActivePlans = activePlans,
                CompletedPlans = completedPlans,
                FailedPlans = failedPlans,
                TotalDaysPlanned = totalDaysPlanned,
                AverageCompletionRate = Math.Round(averageCompletionRate, 2),
                CurrentPlanName = currentPlan?.Name,
                CurrentPlanProgress = Math.Round(currentPlanProgress, 2)
            };
        }

        private async Task CreateInitialSmokingStatusAsync(int accountId, DateTime quitDate, string? quitReason)
        {
            // Check if smoking status already exists
            var existingStatus = await _context.SmokingStatuses
                .FirstOrDefaultAsync(s => s.AccountId == accountId);

            if (existingStatus == null)
            {
                var smokingStatus = new SmokingStatus
                {
                    AccountId = accountId,
                    Status = "QUITTING",
                    QuitDate = quitDate,
                    SmokeFreenDays = 0,
                    CigarettesPerDay = 0, // Will be updated later
                    MoneySaved = 0,
                    HealthImprovement = "Bắt đầu hành trình cai thuốc",
                    LastUpdated = DateTime.UtcNow
                };

                _context.SmokingStatuses.Add(smokingStatus);
                await _context.SaveChangesAsync();
            }
        }

        private string GeneratePlanDescription(MemberPackage package, SmokingStatus? smokingStatus)
        {
            var description = $"Kế hoạch cai thuốc được tạo tự động dựa trên gói thành viên '{package.Name}'.\n\n";
            
            description += $"Thời gian: {package.DurationDays} ngày\n";
            description += $"Giá: {package.Price:C}\n\n";
            
            if (smokingStatus != null)
            {
                description += $"Thông tin hiện tại:\n";
                description += $"- Số điếu thuốc/ngày: {smokingStatus.CigarettesPerDay}\n";
                description += $"- Số ngày đã cai: {smokingStatus.SmokeFreenDays}\n";
                description += $"- Tiền đã tiết kiệm: {smokingStatus.MoneySaved:C}\n\n";
            }
            
            description += "Hãy tuân thủ kế hoạch và cập nhật tiến trình hàng ngày để đạt được kết quả tốt nhất!";
            
            return description;
        }
    }
} 