using SmokingQuitSupportAPI.Models.DTOs.Progress;

namespace SmokingQuitSupportAPI.Services.Interfaces
{
    /// <summary>
    /// Interface cho dịch vụ tiến trình cai thuốc
    /// </summary>
    public interface IProgressService
    {
        /// <summary>
        /// Lấy tất cả tiến trình của người dùng
        /// </summary>
        Task<IEnumerable<ProgressDto>> GetUserProgressAsync(int accountId);
        
        /// <summary>
        /// Lấy tiến trình theo ngày
        /// </summary>
        Task<ProgressDto?> GetProgressByDateAsync(int accountId, DateTime date);
        
        /// <summary>
        /// Lấy tiến trình trong khoảng thời gian
        /// </summary>
        Task<IEnumerable<ProgressDto>> GetProgressByDateRangeAsync(int accountId, DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// Ghi nhận tiến trình hàng ngày
        /// </summary>
        Task<ProgressDto> RecordDailyProgressAsync(int accountId, CreateProgressDto createProgressDto);
        
        /// <summary>
        /// Cập nhật tiến trình
        /// </summary>
        Task<ProgressDto?> UpdateProgressAsync(int progressId, int accountId, CreateProgressDto updateProgressDto);
        
        /// <summary>
        /// Xóa ghi nhận tiến trình
        /// </summary>
        Task<bool> DeleteProgressAsync(int progressId, int accountId);
        
        /// <summary>
        /// Lấy thống kê tiến trình tổng quan
        /// </summary>
        Task<object> GetProgressStatisticsAsync(int accountId);
        
        /// <summary>
        /// Lấy streak (chuỗi ngày liên tiếp không hút thuốc)
        /// </summary>
        Task<int> GetCurrentStreakAsync(int accountId);
        
        /// <summary>
        /// Lấy streak dài nhất
        /// </summary>
        Task<int> GetLongestStreakAsync(int accountId);
        
        /// <summary>
        /// Tính toán và cập nhật thống kê tự động
        /// </summary>
        Task UpdateAutomaticStatisticsAsync(int accountId);
    }
} 