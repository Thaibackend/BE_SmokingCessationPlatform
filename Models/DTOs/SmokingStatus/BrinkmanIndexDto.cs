namespace SmokingQuitSupportAPI.Models.DTOs.SmokingStatus
{
    /// <summary>
    /// DTO cho chỉ số Brinkman Index - Đánh giá nguy cơ mắc bệnh liên quan đến hút thuốc
    /// </summary>
    public class BrinkmanIndexDto
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public int CigarettesPerDay { get; set; }
        public int YearsOfSmoking { get; set; }
        public int BrinkmanIndex { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string RiskColor { get; set; } = string.Empty;
        public string RiskDescription { get; set; } = string.Empty;
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
        
        // Dữ liệu cho biểu đồ sóng
        public List<WaveChartData> WaveData { get; set; } = new List<WaveChartData>();
        
        // Thông tin so sánh với người khác
        public int PercentileRank { get; set; }
        public decimal AverageIndexInSystem { get; set; }
        
        // Khuyến nghị dựa trên chỉ số
        public List<string> HealthRecommendations { get; set; } = new List<string>();
    }

    /// <summary>
    /// Dữ liệu điểm cho biểu đồ sóng Brinkman Index
    /// </summary>
    public class WaveChartData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho thống kê Brinkman Index tổng quan
    /// </summary>
    public class BrinkmanStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int LowRiskUsers { get; set; }
        public int MediumRiskUsers { get; set; }
        public int HighRiskUsers { get; set; }
        public decimal AverageBrinkmanIndex { get; set; }
        public decimal LowRiskPercentage { get; set; }
        public decimal MediumRiskPercentage { get; set; }
        public decimal HighRiskPercentage { get; set; }
        
        // Dữ liệu phân bố cho biểu đồ
        public List<DistributionData> RiskDistribution { get; set; } = new List<DistributionData>();
    }

    /// <summary>
    /// Dữ liệu phân bố nguy cơ
    /// </summary>
    public class DistributionData
    {
        public string RiskLevel { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
    }
} 