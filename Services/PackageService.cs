using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Constants;
using SmokingQuitSupportAPI.Data;
using SmokingQuitSupportAPI.Models.DTOs.Package;
using SmokingQuitSupportAPI.Models.Entities;
using SmokingQuitSupportAPI.Services.Interfaces;

namespace SmokingQuitSupportAPI.Services
{
    /// <summary>
    /// Service xử lý logic quản lý gói dịch vụ
    /// </summary>
    public class PackageService : IPackageService
    {
        private readonly AppDbContext _context;

        public PackageService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tạo subscription Basic mặc định khi user đăng ký
        /// </summary>
        public async Task<UserPackageDto> CreateBasicSubscriptionAsync(int accountId)
        {
            var subscription = new UserSubscription
            {
                AccountId = accountId,
                PackageType = PackageTypes.BASIC,
                Status = SubscriptionStatus.ACTIVE,
                StartDate = DateTime.UtcNow,
                EndDate = null, // Basic không hết hạn
                Price = 0,
                Notes = "Gói Basic mặc định khi đăng ký"
            };

            _context.UserSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return await GetUserPackageAsync(accountId) ?? throw new InvalidOperationException("Failed to create basic subscription");
        }

        /// <summary>
        /// Lấy thông tin package của user
        /// </summary>
        public async Task<UserPackageDto?> GetUserPackageAsync(int accountId)
        {
            var subscription = await _context.UserSubscriptions
                .Include(s => s.Account)
                .Include(s => s.AssignedCoach)
                .ThenInclude(c => c.Account)
                .Where(s => s.AccountId == accountId && s.Status == SubscriptionStatus.ACTIVE)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (subscription == null)
                return null;

            return new UserPackageDto
            {
                SubscriptionId = subscription.SubscriptionId,
                AccountId = subscription.AccountId,
                AccountName = subscription.Account.Username,
                PackageType = subscription.PackageType,
                Status = subscription.Status,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                Price = subscription.Price,
                AssignedCoachId = subscription.AssignedCoachId,
                AssignedCoachName = subscription.AssignedCoach?.Account.Username,
                Notes = subscription.Notes,
                AvailableFeatures = GetAvailableFeatures(subscription.PackageType)
            };
        }

        /// <summary>
        /// Upgrade package lên Premium
        /// </summary>
        public async Task<UserPackageDto> UpgradeToPackageAsync(int accountId, UpgradePackageDto upgradeDto)
        {
            // Deactivate current subscription
            var currentSubscription = await _context.UserSubscriptions
                .Where(s => s.AccountId == accountId && s.Status == SubscriptionStatus.ACTIVE)
                .FirstOrDefaultAsync();

            if (currentSubscription != null)
            {
                currentSubscription.Status = SubscriptionStatus.EXPIRED;
                currentSubscription.UpdatedAt = DateTime.UtcNow;
            }

            // Create new premium subscription
            var newSubscription = new UserSubscription
            {
                AccountId = accountId,
                PackageType = upgradeDto.NewPackageType,
                Status = SubscriptionStatus.ACTIVE,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(upgradeDto.DurationDays),
                Price = upgradeDto.Price,
                AssignedCoachId = upgradeDto.PreferredCoachId,
                Notes = $"Upgraded from {currentSubscription?.PackageType ?? "BASIC"} to {upgradeDto.NewPackageType}"
            };

            _context.UserSubscriptions.Add(newSubscription);

            // Nếu upgrade lên Premium, tạo initial stage progress
            if (upgradeDto.NewPackageType == PackageTypes.PREMIUM)
            {
                await CreateInitialStageProgressAsync(accountId);
            }

            await _context.SaveChangesAsync();

            return await GetUserPackageAsync(accountId) ?? throw new InvalidOperationException("Failed to upgrade package");
        }

        /// <summary>
        /// Tự động đề xuất quit plan cho Basic users
        /// </summary>
        public async Task<SuggestedQuitPlanDto> GenerateSuggestedQuitPlanAsync(int accountId)
        {
            var smokingStatus = await _context.SmokingStatuses
                .Where(s => s.AccountId == accountId)
                .OrderByDescending(s => s.StatusId)
                .FirstOrDefaultAsync();

            if (smokingStatus == null)
            {
                throw new InvalidOperationException("Không tìm thấy thông tin hút thuốc để tạo kế hoạch");
            }

            var brinkmanIndex = smokingStatus.CigarettesPerDay * smokingStatus.YearsOfSmoking;
            
            // Tạo kế hoạch dựa trên Brinkman Index và thói quen hút thuốc
            return GenerateQuitPlanBasedOnProfile(smokingStatus, brinkmanIndex);
        }

        /// <summary>
        /// Kiểm tra user có premium features không
        /// </summary>
        public async Task<bool> HasPremiumAccessAsync(int accountId)
        {
            var subscription = await _context.UserSubscriptions
                .Where(s => s.AccountId == accountId && s.Status == SubscriptionStatus.ACTIVE)
                .FirstOrDefaultAsync();

            return subscription?.PackageType == PackageTypes.PREMIUM && 
                   (!subscription.EndDate.HasValue || subscription.EndDate > DateTime.UtcNow);
        }

        /// <summary>
        /// Tạo initial stage progress cho Premium user
        /// </summary>
        private async Task CreateInitialStageProgressAsync(int accountId)
        {
            var existingProgress = await _context.QuitStageProgresses
                .Where(q => q.AccountId == accountId)
                .FirstOrDefaultAsync();

            if (existingProgress == null)
            {
                var stageProgress = new QuitStageProgress
                {
                    AccountId = accountId,
                    CurrentStage = QuitStages.PREPARATION,
                    StageStartDate = DateTime.UtcNow,
                    ProgressPercentage = 0,
                    StageGoals = "Chuẩn bị tinh thần và môi trường để bắt đầu cai thuốc",
                    UserNotes = "Bắt đầu hành trình cai thuốc với gói Premium"
                };

                _context.QuitStageProgresses.Add(stageProgress);
            }
        }

        /// <summary>
        /// Lấy danh sách features theo package type
        /// </summary>
        private List<string> GetAvailableFeatures(string packageType)
        {
            var features = new List<string>
            {
                "Tính chỉ số Brinkman Index",
                "Thống kê tiến độ cai thuốc",
                "Bảng xếp hạng cộng đồng",
                "Theo dõi tiền tiết kiệm",
                "Kế hoạch cai thuốc tự động"
            };

            if (packageType == PackageTypes.PREMIUM)
            {
                features.AddRange(new[]
                {
                    "Hỗ trợ Coach chuyên nghiệp",
                    "Chat trực tiếp với Coach",
                    "Booking meeting với Coach",
                    "Quản lý chi tiết từng giai đoạn",
                    "Báo cáo tiến độ chi tiết",
                    "Tư vấn cá nhân hóa"
                });
            }

            return features;
        }

        /// <summary>
        /// Tạo quit plan dựa trên profile của user
        /// </summary>
        private SuggestedQuitPlanDto GenerateQuitPlanBasedOnProfile(SmokingStatus smokingStatus, int brinkmanIndex)
        {
            var cigarettesPerDay = smokingStatus.CigarettesPerDay;
            var planDuration = CalculatePlanDuration(cigarettesPerDay, brinkmanIndex);
            var reductionStrategy = GetReductionStrategy(cigarettesPerDay, brinkmanIndex);

            var suggestedPlan = new SuggestedQuitPlanDto
            {
                Name = $"Kế hoạch cai thuốc {planDuration} ngày",
                Description = $"Kế hoạch cai thuốc được tạo tự động dựa trên thói quen hút {cigarettesPerDay} điếu/ngày",
                StartDate = DateTime.UtcNow.Date.AddDays(1), // Bắt đầu từ ngày mai
                EndDate = DateTime.UtcNow.Date.AddDays(planDuration),
                TargetCigarettesPerDay = 0,
                Strategies = GetStrategiesForProfile(brinkmanIndex),
                ReasonForSuggestion = GetReasonForSuggestion(cigarettesPerDay, brinkmanIndex)
            };

            // Tạo milestones
            suggestedPlan.Milestones = CreateMilestones(cigarettesPerDay, planDuration, reductionStrategy);

            return suggestedPlan;
        }

        /// <summary>
        /// Tính thời gian kế hoạch dựa trên mức độ hút thuốc
        /// </summary>
        private int CalculatePlanDuration(int cigarettesPerDay, int brinkmanIndex)
        {
            return brinkmanIndex switch
            {
                < 100 => 21,  // 3 tuần cho nguy cơ thấp
                <= 200 => 42, // 6 tuần cho nguy cơ trung bình
                _ => 84       // 12 tuần cho nguy cơ cao
            };
        }

        /// <summary>
        /// Lấy chiến lược giảm thuốc
        /// </summary>
        private string GetReductionStrategy(int cigarettesPerDay, int brinkmanIndex)
        {
            if (cigarettesPerDay <= 10 && brinkmanIndex < 100)
                return "QUICK"; // Cai nhanh
            else if (cigarettesPerDay <= 20)
                return "GRADUAL"; // Giảm dần
            else
                return "SLOW"; // Giảm từ từ
        }

        /// <summary>
        /// Lấy strategies phù hợp
        /// </summary>
        private List<string> GetStrategiesForProfile(int brinkmanIndex)
        {
            var strategies = new List<string>
            {
                "Đặt ngày cai thuốc cụ thể",
                "Loại bỏ thuốc lá khỏi môi trường sống",
                "Tập thể dục thường xuyên",
                "Tìm hoạt động thay thế"
            };

            if (brinkmanIndex >= 100)
            {
                strategies.AddRange(new[]
                {
                    "Tham khảo ý kiến bác sĩ",
                    "Sử dụng liệu pháp thay thế nicotine",
                    "Tìm nhóm hỗ trợ"
                });
            }

            if (brinkmanIndex > 200)
            {
                strategies.AddRange(new[]
                {
                    "Kiểm tra sức khỏe định kỳ",
                    "Cân nhắc thuốc hỗ trợ cai thuốc",
                    "Theo dõi y tế chặt chẽ"
                });
            }

            return strategies;
        }

        /// <summary>
        /// Tạo milestones cho quit plan
        /// </summary>
        private List<QuitPlanMilestone> CreateMilestones(int cigarettesPerDay, int planDuration, string strategy)
        {
            var milestones = new List<QuitPlanMilestone>();

            if (strategy == "QUICK")
            {
                // Cai nhanh - 3 tuần
                milestones.AddRange(new[]
                {
                    new QuitPlanMilestone
                    {
                        Title = "Tuần 1: Chuẩn bị",
                        Description = "Chuẩn bị tinh thần và môi trường",
                        TargetDate = DateTime.UtcNow.AddDays(7),
                        TargetCigarettes = cigarettesPerDay / 2,
                        Actions = new[] { "Giảm 50% số điếu", "Chuẩn bị tinh thần", "Thông báo với gia đình" }.ToList()
                    },
                    new QuitPlanMilestone
                    {
                        Title = "Tuần 2: Bắt đầu cai",
                        Description = "Ngừng hoàn toàn việc hút thuốc",
                        TargetDate = DateTime.UtcNow.AddDays(14),
                        TargetCigarettes = 0,
                        Actions = new[] { "Ngừng hút thuốc hoàn toàn", "Tập thể dục mỗi ngày", "Uống nhiều nước" }.ToList()
                    },
                    new QuitPlanMilestone
                    {
                        Title = "Tuần 3: Duy trì",
                        Description = "Duy trì và củng cố thói quen mới",
                        TargetDate = DateTime.UtcNow.AddDays(21),
                        TargetCigarettes = 0,
                        Actions = new[] { "Duy trì không hút thuốc", "Tìm hoạt động thay thế", "Thưởng cho bản thân" }.ToList()
                    }
                });
            }
            else if (strategy == "GRADUAL")
            {
                // Giảm dần - 6 tuần
                var weeklyReduction = cigarettesPerDay / 4;
                for (int week = 1; week <= 6; week++)
                {
                    var targetCigarettes = Math.Max(0, cigarettesPerDay - (weeklyReduction * week));
                    milestones.Add(new QuitPlanMilestone
                    {
                        Title = $"Tuần {week}",
                        Description = $"Giảm xuống {targetCigarettes} điếu/ngày",
                        TargetDate = DateTime.UtcNow.AddDays(week * 7),
                        TargetCigarettes = targetCigarettes,
                        Actions = new[] { $"Giảm xuống {targetCigarettes} điếu/ngày", "Ghi nhật ký", "Tập thể dục" }.ToList()
                    });
                }
            }
            else // SLOW
            {
                // Giảm từ từ - 12 tuần
                var weeklyReduction = cigarettesPerDay / 8;
                for (int week = 1; week <= 12; week++)
                {
                    var targetCigarettes = Math.Max(0, cigarettesPerDay - (weeklyReduction * week));
                    milestones.Add(new QuitPlanMilestone
                    {
                        Title = $"Tuần {week}",
                        Description = $"Mục tiêu: {targetCigarettes} điếu/ngày",
                        TargetDate = DateTime.UtcNow.AddDays(week * 7),
                        TargetCigarettes = targetCigarettes,
                        Actions = new[] { "Giảm từ từ", "Theo dõi cảm xúc", "Tìm hỗ trợ" }.ToList()
                    });
                }
            }

            return milestones;
        }

        /// <summary>
        /// Lấy lý do đề xuất plan
        /// </summary>
        private string GetReasonForSuggestion(int cigarettesPerDay, int brinkmanIndex)
        {
            return brinkmanIndex switch
            {
                < 100 => $"Với {cigarettesPerDay} điếu/ngày và chỉ số Brinkman {brinkmanIndex}, bạn có thể cai thuốc nhanh chóng với kế hoạch tập trung.",
                <= 200 => $"Chỉ số Brinkman {brinkmanIndex} cho thấy bạn cần kế hoạch giảm dần để giảm thiểu tác dụng phụ khi cai thuốc.",
                _ => $"Với chỉ số Brinkman cao ({brinkmanIndex}), kế hoạch cai từ từ sẽ giúp bạn thành công và an toàn hơn."
            };
        }
    }
} 