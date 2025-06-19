namespace SmokingQuitSupportAPI.Constants
{
    /// <summary>
    /// Các loại gói dịch vụ trong hệ thống
    /// </summary>
    public static class PackageTypes
    {
        /// <summary>
        /// Gói cơ bản - Miễn phí
        /// </summary>
        public const string BASIC = "BASIC";
        
        /// <summary>
        /// Gói cao cấp - Có phí
        /// </summary>
        public const string PREMIUM = "PREMIUM";
    }

    /// <summary>
    /// Trạng thái subscription
    /// </summary>
    public static class SubscriptionStatus
    {
        public const string ACTIVE = "ACTIVE";
        public const string EXPIRED = "EXPIRED";
        public const string CANCELLED = "CANCELLED";
        public const string PENDING = "PENDING";
    }

    /// <summary>
    /// Các giai đoạn cai thuốc (cho Premium users)
    /// </summary>
    public static class QuitStages
    {
        public const string PREPARATION = "PREPARATION";     // Chuẩn bị
        public const string INITIAL_QUIT = "INITIAL_QUIT";   // Bắt đầu cai (0-3 ngày)
        public const string EARLY_RECOVERY = "EARLY_RECOVERY"; // Phục hồi sớm (4-30 ngày)
        public const string ONGOING_RECOVERY = "ONGOING_RECOVERY"; // Phục hồi tiếp tục (1-12 tháng)
        public const string MAINTENANCE = "MAINTENANCE";     // Duy trì (12+ tháng)
    }

    /// <summary>
    /// Các loại meeting với coach
    /// </summary>
    public static class MeetingTypes
    {
        public const string INITIAL_CONSULTATION = "INITIAL_CONSULTATION";
        public const string FOLLOW_UP = "FOLLOW_UP";
        public const string CRISIS_SUPPORT = "CRISIS_SUPPORT";
        public const string PROGRESS_REVIEW = "PROGRESS_REVIEW";
    }
} 