namespace SmokingQuitSupportAPI.Models.DTOs.Package
{
    /// <summary>
    /// DTO cho thông tin gói dịch vụ của user
    /// </summary>
    public class UserPackageDto
    {
        public int SubscriptionId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string PackageType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Price { get; set; }
        public int? AssignedCoachId { get; set; }
        public string? AssignedCoachName { get; set; }
        public string? Notes { get; set; }
        
        // Thông tin thêm
        public bool IsPremium => PackageType == "PREMIUM";
        public bool IsActive => Status == "ACTIVE";
        public int DaysRemaining => EndDate.HasValue ? Math.Max(0, (EndDate.Value - DateTime.UtcNow).Days) : 0;
        
        // Features available
        public List<string> AvailableFeatures { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO cho upgrade package
    /// </summary>
    public class UpgradePackageDto
    {
        public int AccountId { get; set; }
        public string NewPackageType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public int? PreferredCoachId { get; set; }
    }

    /// <summary>
    /// DTO cho suggested quit plan
    /// </summary>
    public class SuggestedQuitPlanDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TargetCigarettesPerDay { get; set; }
        public List<string> Strategies { get; set; } = new List<string>();
        public List<QuitPlanMilestone> Milestones { get; set; } = new List<QuitPlanMilestone>();
        public string ReasonForSuggestion { get; set; } = string.Empty;
    }

    /// <summary>
    /// Milestone trong quit plan
    /// </summary>
    public class QuitPlanMilestone
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime TargetDate { get; set; }
        public int TargetCigarettes { get; set; }
        public List<string> Actions { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO cho stage progress (Premium)
    /// </summary>
    public class QuitStageProgressDto
    {
        public int StageProgressId { get; set; }
        public int AccountId { get; set; }
        public string CurrentStage { get; set; } = string.Empty;
        public string StageDisplayName { get; set; } = string.Empty;
        public string StageDescription { get; set; } = string.Empty;
        public DateTime StageStartDate { get; set; }
        public DateTime? StageEndDate { get; set; }
        public int ProgressPercentage { get; set; }
        public string? StageGoals { get; set; }
        public string? CoachNotes { get; set; }
        public string? UserNotes { get; set; }
        public int? CigarettesSmoked { get; set; }
        public int? CravingLevel { get; set; }
        public int? StressLevel { get; set; }
        public int? SupportLevel { get; set; }
        public string? Challenges { get; set; }
        public string? Achievements { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Computed properties
        public int DaysInStage => (DateTime.UtcNow - StageStartDate).Days;
        public string NextStage { get; set; } = string.Empty;
        public List<string> NextStageActions { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO cho chat message
    /// </summary>
    public class ChatMessageDto
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? MessageType { get; set; }
        public string? AttachmentUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime SentAt { get; set; }
    }

    /// <summary>
    /// DTO để tạo chat message
    /// </summary>
    public class CreateChatMessageDto
    {
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? MessageType { get; set; } = "TEXT";
        public string? AttachmentUrl { get; set; }
    }

    /// <summary>
    /// DTO cho booking meeting với coach
    /// </summary>
    public class BookMeetingDto
    {
        public DateTime PreferredDate { get; set; }
        public string MeetingType { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin meeting
    /// </summary>
    public class MeetingDto
    {
        public int SessionId { get; set; }
        public int CoachId { get; set; }
        public string CoachName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string SessionType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO để cập nhật stage progress
    /// </summary>
    public class UpdateStageProgressDto
    {
        public int ProgressPercentage { get; set; }
        public string? UserNotes { get; set; }
        public int? CigarettesSmoked { get; set; }
        public int? CravingLevel { get; set; }
        public int? StressLevel { get; set; }
        public int? SupportLevel { get; set; }
        public string? Challenges { get; set; }
        public string? Achievements { get; set; }
    }
} 