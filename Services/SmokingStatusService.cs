using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data;
using SmokingQuitSupportAPI.Models.DTOs.SmokingStatus;
using SmokingQuitSupportAPI.Models.Entities;
using SmokingQuitSupportAPI.Services.Interfaces;

namespace SmokingQuitSupportAPI.Services
{
    /// <summary>
    /// Service xử lý logic tình trạng hút thuốc
    /// </summary>
    public class SmokingStatusService : ISmokingStatusService
    {
        private readonly AppDbContext _context;

        public SmokingStatusService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SmokingStatusDto?> GetSmokingStatusAsync(int accountId)
        {
            var smokingStatus = await _context.SmokingStatuses
                .Include(s => s.Account)
                .Where(s => s.AccountId == accountId)
                .OrderByDescending(s => s.StatusId)
                .Select(s => new SmokingStatusDto
                {
                    StatusId = s.StatusId,
                    AccountId = s.AccountId,
                    QuitDate = s.QuitDate,
                    CigarettesPerDay = s.CigarettesPerDay,
                    YearsOfSmoking = s.YearsOfSmoking,
                    CostPerPack = s.CostPerPack,
                    CigarettesPerPack = s.CigarettesPerPack,
                    MoneySaved = s.MoneySaved,
                    SmokeFreenDays = s.SmokeFreenDays,
                    CigarettesAvoided = s.CigarettesAvoided,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.LastUpdated,
                    AccountName = s.Account.Username
                })
                .FirstOrDefaultAsync();

            return smokingStatus;
        }

        public async Task<SmokingStatusDto> CreateOrUpdateSmokingStatusAsync(int accountId, CreateSmokingStatusDto createStatusDto)
        {
            var existingStatus = await _context.SmokingStatuses
                .FirstOrDefaultAsync(s => s.AccountId == accountId);

            if (existingStatus == null)
            {
                // Create new smoking status
                var newStatus = new SmokingStatus
                {
                    AccountId = accountId,
                    QuitDate = createStatusDto.QuitDate,
                    CigarettesPerDay = createStatusDto.CigarettesPerDay,
                    YearsOfSmoking = createStatusDto.YearsOfSmoking,
                    CostPerPack = createStatusDto.CostPerPack,
                    CigarettesPerPack = createStatusDto.CigarettesPerPack,
                    Status = "ACTIVE",
                    SmokeFreenDays = CalculateSmokeFreenDays(createStatusDto.QuitDate),
                    CigarettesAvoided = CalculateCigarettesAvoided(createStatusDto.QuitDate, createStatusDto.CigarettesPerDay),
                    MoneySaved = CalculateMoneySaved(createStatusDto.QuitDate, createStatusDto.CigarettesPerDay, createStatusDto.CostPerPack, createStatusDto.CigarettesPerPack),
                    HealthImprovement = GenerateHealthImprovement(CalculateSmokeFreenDays(createStatusDto.QuitDate)),
                    CreatedAt = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                };

                _context.SmokingStatuses.Add(newStatus);
                await _context.SaveChangesAsync();

                return await GetSmokingStatusAsync(accountId) ?? throw new InvalidOperationException("Failed to create smoking status");
            }
            else
            {
                // Update existing status
                existingStatus.QuitDate = createStatusDto.QuitDate;
                existingStatus.CigarettesPerDay = createStatusDto.CigarettesPerDay;
                existingStatus.YearsOfSmoking = createStatusDto.YearsOfSmoking;
                existingStatus.CostPerPack = createStatusDto.CostPerPack;
                existingStatus.CigarettesPerPack = createStatusDto.CigarettesPerPack;
                existingStatus.SmokeFreenDays = CalculateSmokeFreenDays(createStatusDto.QuitDate);
                existingStatus.CigarettesAvoided = CalculateCigarettesAvoided(createStatusDto.QuitDate, createStatusDto.CigarettesPerDay);
                existingStatus.MoneySaved = CalculateMoneySaved(createStatusDto.QuitDate, createStatusDto.CigarettesPerDay, createStatusDto.CostPerPack, createStatusDto.CigarettesPerPack);
                existingStatus.HealthImprovement = GenerateHealthImprovement(existingStatus.SmokeFreenDays);
                existingStatus.LastUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return await GetSmokingStatusAsync(accountId) ?? throw new InvalidOperationException("Failed to update smoking status");
            }
        }

        public async Task UpdateProgressStatisticsAsync(int accountId)
        {
            var smokingStatus = await _context.SmokingStatuses
                .FirstOrDefaultAsync(s => s.AccountId == accountId);

            if (smokingStatus == null)
                return;

            // Tính toán lại các thống kê
            smokingStatus.SmokeFreenDays = CalculateSmokeFreenDays(smokingStatus.QuitDate);
            smokingStatus.CigarettesAvoided = CalculateCigarettesAvoided(smokingStatus.QuitDate, smokingStatus.CigarettesPerDay);
            smokingStatus.MoneySaved = CalculateMoneySaved(smokingStatus.QuitDate, smokingStatus.CigarettesPerDay, smokingStatus.CostPerPack, smokingStatus.CigarettesPerPack);
            smokingStatus.HealthImprovement = GenerateHealthImprovement(smokingStatus.SmokeFreenDays);
            smokingStatus.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<object> GetOverallStatisticsAsync()
        {
            var allStatuses = await _context.SmokingStatuses
                .Include(s => s.Account)
                .ToListAsync();

            if (!allStatuses.Any())
            {
                return new
                {
                    TotalUsers = 0,
                    ActiveUsers = 0,
                    TotalDaysSmokeFree = 0,
                    TotalCigarettesAvoided = 0,
                    TotalMoneySaved = 0m,
                    AverageQuitDays = 0.0,
                    SuccessRate = 0.0
                };
            }

            var activeUsers = allStatuses.Count(s => s.Status == "ACTIVE");
            var totalDaysSmokeFree = allStatuses.Sum(s => s.SmokeFreenDays);
            var totalCigarettesAvoided = allStatuses.Sum(s => s.CigarettesAvoided);
            var totalMoneySaved = allStatuses.Sum(s => s.MoneySaved);
            var averageQuitDays = allStatuses.Average(s => s.SmokeFreenDays);
            
            // Tính success rate dựa trên số user đã cai được trên 30 ngày
            var successfulUsers = allStatuses.Count(s => s.SmokeFreenDays >= 30);
            var successRate = (double)successfulUsers / allStatuses.Count * 100;

            return new
            {
                TotalUsers = allStatuses.Count,
                ActiveUsers = activeUsers,
                TotalDaysSmokeFree = totalDaysSmokeFree,
                TotalCigarettesAvoided = totalCigarettesAvoided,
                TotalMoneySaved = totalMoneySaved,
                AverageQuitDays = Math.Round(averageQuitDays, 1),
                SuccessRate = Math.Round(successRate, 2)
            };
        }

        public async Task<IEnumerable<SmokingStatusDto>> GetLeaderboardByMoneySavedAsync(int take = 10)
        {
            var leaderboard = await _context.SmokingStatuses
                .Include(s => s.Account)
                .OrderByDescending(s => s.MoneySaved)
                .Take(take)
                .Select(s => new SmokingStatusDto
                {
                    StatusId = s.StatusId,
                    AccountId = s.AccountId,
                    QuitDate = s.QuitDate,
                    CigarettesPerDay = s.CigarettesPerDay,
                    YearsOfSmoking = s.YearsOfSmoking,
                    CostPerPack = s.CostPerPack,
                    CigarettesPerPack = s.CigarettesPerPack,
                    MoneySaved = s.MoneySaved,
                    SmokeFreenDays = s.SmokeFreenDays,
                    CigarettesAvoided = s.CigarettesAvoided,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.LastUpdated,
                    AccountName = s.Account.Username
                })
                .ToListAsync();

            return leaderboard;
        }

        public async Task<IEnumerable<SmokingStatusDto>> GetLeaderboardBySmokeFreesDaysAsync(int take = 10)
        {
            var leaderboard = await _context.SmokingStatuses
                .Include(s => s.Account)
                .OrderByDescending(s => s.SmokeFreenDays)
                .Take(take)
                .Select(s => new SmokingStatusDto
                {
                    StatusId = s.StatusId,
                    AccountId = s.AccountId,
                    QuitDate = s.QuitDate,
                    CigarettesPerDay = s.CigarettesPerDay,
                    YearsOfSmoking = s.YearsOfSmoking,
                    CostPerPack = s.CostPerPack,
                    CigarettesPerPack = s.CigarettesPerPack,
                    MoneySaved = s.MoneySaved,
                    SmokeFreenDays = s.SmokeFreenDays,
                    CigarettesAvoided = s.CigarettesAvoided,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.LastUpdated,
                    AccountName = s.Account.Username
                })
                .ToListAsync();

            return leaderboard;
        }

        /// <summary>
        /// Lấy chỉ số Brinkman Index của người dùng với dữ liệu biểu đồ sóng
        /// </summary>
        public async Task<BrinkmanIndexDto?> GetBrinkmanIndexAsync(int accountId)
        {
            var smokingStatus = await _context.SmokingStatuses
                .Include(s => s.Account)
                .Where(s => s.AccountId == accountId)
                .OrderByDescending(s => s.StatusId)
                .FirstOrDefaultAsync();

            if (smokingStatus == null)
                return null;

            var brinkmanIndex = smokingStatus.CigarettesPerDay * smokingStatus.YearsOfSmoking;
            var allStatuses = await _context.SmokingStatuses.ToListAsync();
            var averageIndex = allStatuses.Any() ? (decimal)allStatuses.Average(s => s.CigarettesPerDay * s.YearsOfSmoking) : 0;
            
            // Tính percentile rank
            var lowerIndexCount = allStatuses.Count(s => (s.CigarettesPerDay * s.YearsOfSmoking) < brinkmanIndex);
            var percentileRank = allStatuses.Count > 0 ? (int)Math.Round((double)lowerIndexCount / allStatuses.Count * 100) : 0;

            return new BrinkmanIndexDto
            {
                AccountId = smokingStatus.AccountId,
                AccountName = smokingStatus.Account.Username,
                CigarettesPerDay = smokingStatus.CigarettesPerDay,
                YearsOfSmoking = smokingStatus.YearsOfSmoking,
                BrinkmanIndex = brinkmanIndex,
                RiskLevel = GetRiskLevel(brinkmanIndex),
                RiskColor = GetRiskColor(brinkmanIndex),
                RiskDescription = GetRiskDescription(brinkmanIndex),
                PercentileRank = percentileRank,
                AverageIndexInSystem = averageIndex,
                WaveData = GenerateWaveChartData(brinkmanIndex),
                HealthRecommendations = GetHealthRecommendations(brinkmanIndex),
                CalculatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Lấy thống kê Brinkman Index tổng quan
        /// </summary>
        public async Task<BrinkmanStatisticsDto> GetBrinkmanStatisticsAsync()
        {
            var allStatuses = await _context.SmokingStatuses.ToListAsync();
            
            if (!allStatuses.Any())
            {
                return new BrinkmanStatisticsDto
                {
                    TotalUsers = 0,
                    LowRiskUsers = 0,
                    MediumRiskUsers = 0,
                    HighRiskUsers = 0,
                    AverageBrinkmanIndex = 0,
                    LowRiskPercentage = 0,
                    MediumRiskPercentage = 0,
                    HighRiskPercentage = 0,
                    RiskDistribution = new List<DistributionData>()
                };
            }

            var lowRiskUsers = allStatuses.Count(s => (s.CigarettesPerDay * s.YearsOfSmoking) < 100);
            var mediumRiskUsers = allStatuses.Count(s => 
                (s.CigarettesPerDay * s.YearsOfSmoking) >= 100 && 
                (s.CigarettesPerDay * s.YearsOfSmoking) <= 200);
            var highRiskUsers = allStatuses.Count(s => (s.CigarettesPerDay * s.YearsOfSmoking) > 200);
            var totalUsers = allStatuses.Count;
            var averageIndex = (decimal)allStatuses.Average(s => s.CigarettesPerDay * s.YearsOfSmoking);

            return new BrinkmanStatisticsDto
            {
                TotalUsers = totalUsers,
                LowRiskUsers = lowRiskUsers,
                MediumRiskUsers = mediumRiskUsers,
                HighRiskUsers = highRiskUsers,
                AverageBrinkmanIndex = averageIndex,
                LowRiskPercentage = Math.Round((decimal)lowRiskUsers / totalUsers * 100, 2),
                MediumRiskPercentage = Math.Round((decimal)mediumRiskUsers / totalUsers * 100, 2),
                HighRiskPercentage = Math.Round((decimal)highRiskUsers / totalUsers * 100, 2),
                RiskDistribution = new List<DistributionData>
                {
                    new DistributionData
                    {
                        RiskLevel = "Nguy cơ thấp",
                        Count = lowRiskUsers,
                        Percentage = Math.Round((decimal)lowRiskUsers / totalUsers * 100, 2),
                        Color = "#28a745"
                    },
                    new DistributionData
                    {
                        RiskLevel = "Nguy cơ trung bình",
                        Count = mediumRiskUsers,
                        Percentage = Math.Round((decimal)mediumRiskUsers / totalUsers * 100, 2),
                        Color = "#ffc107"
                    },
                    new DistributionData
                    {
                        RiskLevel = "Nguy cơ cao",
                        Count = highRiskUsers,
                        Percentage = Math.Round((decimal)highRiskUsers / totalUsers * 100, 2),
                        Color = "#dc3545"
                    }
                }
            };
        }

        /// <summary>
        /// Tính số ngày đã cai thuốc
        /// </summary>
        private int CalculateSmokeFreenDays(DateTime quitDate)
        {
            var days = (DateTime.UtcNow.Date - quitDate.Date).Days;
            return Math.Max(0, days);
        }

        /// <summary>
        /// Tính số điếu thuốc đã tránh được
        /// </summary>
        private int CalculateCigarettesAvoided(DateTime quitDate, int cigarettesPerDay)
        {
            var smokeFreenDays = CalculateSmokeFreenDays(quitDate);
            return smokeFreenDays * cigarettesPerDay;
        }

        /// <summary>
        /// Tính tiền đã tiết kiệm được
        /// </summary>
        private decimal CalculateMoneySaved(DateTime quitDate, int cigarettesPerDay, decimal costPerPack, int cigarettesPerPack)
        {
            var cigarettesAvoided = CalculateCigarettesAvoided(quitDate, cigarettesPerDay);
            var costPerCigarette = costPerPack / cigarettesPerPack;
            return cigarettesAvoided * costPerCigarette;
        }

        /// <summary>
        /// Tạo thông điệp cải thiện sức khỏe dựa trên số ngày cai thuốc
        /// </summary>
        private string GenerateHealthImprovement(int smokeFreenDays)
        {
            return smokeFreenDays switch
            {
                0 => "Bắt đầu hành trình cai thuốc - Cơ thể bắt đầu thanh lọc nicotine",
                1 => "Ngày đầu tiên thành công! Nguy cơ đau tim bắt đầu giảm",
                7 => "1 tuần không thuốc! Vị giác và khứu giác đang cải thiện",
                14 => "2 tuần! Tuần hoàn máu và chức năng phổi đang cải thiện",
                30 => "1 tháng! Nguy cơ nhiễm trùng giảm đáng kể",
                90 => "3 tháng! Chức năng phổi cải thiện đến 30%",
                365 => "1 năm! Nguy cơ bệnh tim giảm 50%",
                _ => smokeFreenDays < 7 ? "Tiếp tục cố gắng! Cơ thể đang phục hồi" :
                     smokeFreenDays < 30 ? "Chức năng phổi đang cải thiện từng ngày" :
                     smokeFreenDays < 365 ? "Sức khỏe tổng thể đang được cải thiện đáng kể" :
                     "Chúc mừng! Bạn đã đạt được thành tựu tuyệt vời cho sức khỏe"
            };
        }

        /// <summary>
        /// Xác định mức độ nguy cơ dựa trên Brinkman Index
        /// </summary>
        private string GetRiskLevel(int brinkmanIndex)
        {
            return brinkmanIndex < 100 ? "Nguy cơ thấp" :
                   brinkmanIndex <= 200 ? "Nguy cơ trung bình" : "Nguy cơ cao";
        }

        /// <summary>
        /// Lấy màu sắc tương ứng với mức độ nguy cơ
        /// </summary>
        private string GetRiskColor(int brinkmanIndex)
        {
            return brinkmanIndex < 100 ? "#28a745" :
                   brinkmanIndex <= 200 ? "#ffc107" : "#dc3545";
        }

        /// <summary>
        /// Lấy mô tả chi tiết về mức độ nguy cơ
        /// </summary>
        private string GetRiskDescription(int brinkmanIndex)
        {
            return brinkmanIndex switch
            {
                < 100 => "Chỉ số hút thuốc của bạn ở mức thấp. Tuy nhiên, việc bỏ thuốc hoàn toàn vẫn rất quan trọng để bảo vệ sức khỏe lâu dài.",
                <= 200 => "Chỉ số hút thuốc của bạn ở mức trung bình. Bạn có nguy cơ mắc các bệnh liên quan đến thuốc lá. Hãy nghiêm túc xem xét việc bỏ thuốc.",
                _ => "Chỉ số hút thuốc của bạn ở mức cao. Bạn có nguy cơ rất cao mắc các bệnh nghiêm trọng như ung thư phổi, bệnh tim mạch. Cần bỏ thuốc ngay lập tức."
            };
        }

        /// <summary>
        /// Tạo dữ liệu biểu đồ sóng cho Brinkman Index
        /// </summary>
        private List<WaveChartData> GenerateWaveChartData(int brinkmanIndex)
        {
            var waveData = new List<WaveChartData>();
            var maxX = 400; // Maximum Brinkman Index for chart
            var steps = 100; // Number of data points
            
            for (int i = 0; i <= steps; i++)
            {
                var x = (double)i / steps * maxX;
                var y = CalculateWaveValue(x, brinkmanIndex);
                var color = GetColorForIndex((int)x);
                var label = x == brinkmanIndex ? "Chỉ số của bạn" : "";
                
                waveData.Add(new WaveChartData
                {
                    X = x,
                    Y = y,
                    Color = color,
                    Label = label
                });
            }
            
            return waveData;
        }

        /// <summary>
        /// Tính giá trị sóng dựa trên Brinkman Index
        /// </summary>
        private double CalculateWaveValue(double x, int userIndex)
        {
            // Create a wave pattern that peaks at user's index
            var wave1 = Math.Sin(x * Math.PI / 100) * 0.3;
            var wave2 = Math.Sin(x * Math.PI / 50) * 0.2;
            var gaussian = Math.Exp(-Math.Pow(x - userIndex, 2) / (2 * Math.Pow(50, 2))) * 0.8;
            
            return Math.Max(0, wave1 + wave2 + gaussian);
        }

        /// <summary>
        /// Lấy màu sắc cho điểm dữ liệu dựa trên chỉ số
        /// </summary>
        private string GetColorForIndex(int index)
        {
            return index < 100 ? "#28a745" :
                   index <= 200 ? "#ffc107" : "#dc3545";
        }

        /// <summary>
        /// Lấy khuyến nghị sức khỏe dựa trên Brinkman Index
        /// </summary>
        private List<string> GetHealthRecommendations(int brinkmanIndex)
        {
            var recommendations = new List<string>();

            if (brinkmanIndex < 100)
            {
                recommendations.AddRange(new[]
                {
                    "Bỏ thuốc hoàn toàn để bảo vệ sức khỏe lâu dài",
                    "Duy trì lối sống lành mạnh với chế độ ăn uống cân bằng",
                    "Tập thể dục thường xuyên để tăng cường sức khỏe phổi",
                    "Tránh tiếp xúc với khói thuốc thụ động"
                });
            }
            else if (brinkmanIndex <= 200)
            {
                recommendations.AddRange(new[]
                {
                    "BỎ THUỐC NGAY LẬP TỨC - Nguy cơ sức khỏe đang gia tăng",
                    "Tham khảo ý kiến bác sĩ về kế hoạch cai thuốc phù hợp",
                    "Sử dụng liệu pháp thay thế nicotine nếu cần thiết",
                    "Tham gia nhóm hỗ trợ cai thuốc",
                    "Khám sức khỏe định kỳ, đặc biệt kiểm tra phổi và tim mạch"
                });
            }
            else
            {
                recommendations.AddRange(new[]
                {
                    "KHẨN CẤP: Bỏ thuốc ngay lập tức và tìm sự hỗ trợ y tế",
                    "Đến cơ sở y tế để được tư vấn và hỗ trợ chuyên nghiệp",
                    "Kiểm tra sức khỏe toàn diện ngay lập tức",
                    "Xem xét sử dụng thuốc hỗ trợ cai thuốc theo chỉ định của bác sĩ",
                    "Thay đổi hoàn toàn lối sống: ăn uống lành mạnh, tập thể dục",
                    "Tránh xa môi trường có khói thuốc",
                    "Theo dõi các triệu chứng bất thường và báo ngay với bác sĩ"
                });
            }

            return recommendations;
        }
    }
} 