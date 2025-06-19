namespace SmokingQuitSupportAPI.Models.DTOs.QuitPlan
{
    /// <summary>
    /// DTO cho kế hoạch cai thuốc
    /// </summary>
    public class QuitPlanDto
    {
        public int PlanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PackageId { get; set; }
        public int MemberId { get; set; }
        public int? CoachId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Thông tin liên quan
        public string PackageName { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public string? CoachName { get; set; }
        
        // Thống kê tiến độ
        public int DaysInPlan => EndDate.HasValue ? (EndDate.Value - StartDate).Days : 0;
        public int DaysCompleted => (DateTime.UtcNow - StartDate).Days;
        public double CompletionPercentage => DaysInPlan > 0 ? Math.Min(100, (double)DaysCompleted / DaysInPlan * 100) : 0;
    }
} 