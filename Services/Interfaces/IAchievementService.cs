using SmokingQuitSupportAPI.Models.DTOs.Achievement;

namespace SmokingQuitSupportAPI.Services.Interfaces
{
    /// <summary>
    /// Interface cho dịch vụ huy hiệu thành tích
    /// </summary>
    public interface IAchievementService
    {
        /// <summary>
        /// Lấy tất cả huy hiệu thành tích
        /// </summary>
        Task<IEnumerable<AchievementDto>> GetAllAchievementsAsync();
        
        /// <summary>
        /// Lấy huy hiệu thành tích của người dùng
        /// </summary>
        Task<IEnumerable<AchievementDto>> GetUserAchievementsAsync(int accountId);
        
        /// <summary>
        /// Lấy huy hiệu thành tích đã mở khóa của người dùng
        /// </summary>
        Task<IEnumerable<AchievementDto>> GetUnlockedAchievementsAsync(int accountId);
        
        /// <summary>
        /// Kiểm tra và mở khóa huy hiệu thành tích mới
        /// </summary>
        Task<IEnumerable<AchievementDto>> CheckAndUnlockAchievementsAsync(int accountId);
        
        /// <summary>
        /// Mở khóa huy hiệu thành tích cụ thể
        /// </summary>
        Task<bool> UnlockAchievementAsync(int accountId, int achievementId);
        
        /// <summary>
        /// Lấy tiến trình của huy hiệu thành tích
        /// </summary>
        Task<AchievementDto?> GetAchievementProgressAsync(int accountId, int achievementId);
        
        /// <summary>
        /// Lấy bảng xếp hạng theo số huy hiệu
        /// </summary>
        Task<IEnumerable<object>> GetAchievementLeaderboardAsync(int take = 10);
        
        /// <summary>
        /// Chia sẻ huy hiệu thành tích
        /// </summary>
        Task<bool> ShareAchievementAsync(int accountId, int achievementId);
    }
} 