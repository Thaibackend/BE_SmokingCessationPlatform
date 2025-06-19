namespace SmokingQuitSupportAPI.Models.DTOs.Progress
{
    /// <summary>
    /// DTO cho tiến trình cai thuốc - Ghi nhận và thống kê tiến độ
    /// </summary>
    public class ProgressDto
    {
        public int ProgressId { get; set; }
        public int AccountId { get; set; }
        public DateTime Date { get; set; }
        public int SmokeFreenDays { get; set; }
        public int CigarettesAvoided { get; set; }
        public decimal MoneySaved { get; set; }
        public int? HealthScore { get; set; }
        public string? Notes { get; set; }
        public int? Mood { get; set; }
        public int? CravingLevel { get; set; }
        public decimal? Weight { get; set; }
        public int? ExerciseMinutes { get; set; }
        public decimal? SleepHours { get; set; }
        
        // Thông tin tài khoản
        public string AccountName { get; set; } = string.Empty;
        
        // Đánh giá tiến độ
        public string MoodDescription => Mood switch
        {
            1 => "Rất tồi tệ",
            2 => "Tồi tệ", 
            3 => "Bình thường",
            4 => "Tốt",
            5 => "Rất tốt",
            _ => "Chưa đánh giá"
        };
        
        public string CravingDescription => CravingLevel switch
        {
            1 => "Không có cảm giác thèm",
            2 => "Hơi thèm",
            3 => "Thèm vừa phải", 
            4 => "Thèm nhiều",
            5 => "Rất thèm",
            _ => "Chưa đánh giá"
        };

        // Backward compatibility properties for existing controllers
        public bool IsSmokeFreeDay => CigarettesAvoided > 0;
        public int CigarettesSmoked => 0; // Assuming all days are smoke-free in progress tracking
        public int? StressLevel => CravingLevel; // Map craving level to stress level
    }
} 