namespace SmokingQuitSupportAPI.Models.DTOs.Achievement
{
    /// <summary>
    /// DTO cho huy hiệu thành tích
    /// </summary>
    public class AchievementDto
    {
        public int AchievementId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string Type { get; set; } = string.Empty;
        public int RequiredValue { get; set; }
        public string? BadgeColor { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Thông tin cho user hiện tại
        public bool IsUnlocked { get; set; }
        public DateTime? UnlockedAt { get; set; }
        public int CurrentProgress { get; set; }
        public double ProgressPercentage => RequiredValue > 0 ? Math.Min(100, (double)CurrentProgress / RequiredValue * 100) : 0;
        
        // Thống kê
        public int TotalUnlocks { get; set; }
    }
} 