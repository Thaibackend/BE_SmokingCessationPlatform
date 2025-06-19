namespace SmokingQuitSupportAPI.Models.DTOs.SmokingStatus
{
    /// <summary>
    /// DTO cho tình trạng hút thuốc - Ghi nhận thông tin hút thuốc hiện tại
    /// </summary>
    public class SmokingStatusDto
    {
        public int StatusId { get; set; }
        public int AccountId { get; set; }
        public DateTime QuitDate { get; set; }
        public int CigarettesPerDay { get; set; }
        public int YearsOfSmoking { get; set; }
        public decimal CostPerPack { get; set; }
        public int CigarettesPerPack { get; set; }
        public decimal MoneySaved { get; set; }
        public int SmokeFreenDays { get; set; }
        public int CigarettesAvoided { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Thông tin tài khoản
        public string AccountName { get; set; } = string.Empty;
        
        // Tính toán thống kê
        public decimal DailySavings => (decimal)CigarettesPerDay / CigarettesPerPack * CostPerPack;
        public decimal TotalPotentialSavings => SmokeFreenDays * DailySavings;
        public TimeSpan TimeSinceQuit => DateTime.UtcNow - QuitDate;
        
        // Tính toán Brinkman Index
        public int BrinkmanIndex => CigarettesPerDay * YearsOfSmoking;
        public string RiskLevel => BrinkmanIndex < 100 ? "Nguy cơ thấp" : 
                                   BrinkmanIndex <= 200 ? "Nguy cơ trung bình" : "Nguy cơ cao";
        public string RiskColor => BrinkmanIndex < 100 ? "#28a745" : 
                                   BrinkmanIndex <= 200 ? "#ffc107" : "#dc3545";
    }
} 