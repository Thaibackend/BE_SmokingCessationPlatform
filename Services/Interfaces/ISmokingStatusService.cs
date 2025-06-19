using SmokingQuitSupportAPI.Models.DTOs.SmokingStatus;

namespace SmokingQuitSupportAPI.Services.Interfaces
{
    /// <summary>
    /// Interface cho dịch vụ tình trạng hút thuốc
    /// </summary>
    public interface ISmokingStatusService
    {
        /// <summary>
        /// Lấy tình trạng hút thuốc của người dùng
        /// </summary>
        Task<SmokingStatusDto?> GetSmokingStatusAsync(int accountId);
        
        /// <summary>
        /// Tạo hoặc cập nhật tình trạng hút thuốc
        /// </summary>
        Task<SmokingStatusDto> CreateOrUpdateSmokingStatusAsync(int accountId, CreateSmokingStatusDto createStatusDto);
        
        /// <summary>
        /// Cập nhật thống kê tiến trình cai thuốc
        /// </summary>
        Task UpdateProgressStatisticsAsync(int accountId);
        
        /// <summary>
        /// Lấy thống kê tổng quan của tất cả người dùng
        /// </summary>
        Task<object> GetOverallStatisticsAsync();
        
        /// <summary>
        /// Lấy bảng xếp hạng theo tiền tiết kiệm được
        /// </summary>
        Task<IEnumerable<SmokingStatusDto>> GetLeaderboardByMoneySavedAsync(int take = 10);
        
        /// <summary>
        /// Lấy bảng xếp hạng theo số ngày không hút thuốc
        /// </summary>
        Task<IEnumerable<SmokingStatusDto>> GetLeaderboardBySmokeFreesDaysAsync(int take = 10);
        
        /// <summary>
        /// Lấy chỉ số Brinkman Index của người dùng với dữ liệu biểu đồ sóng
        /// </summary>
        Task<BrinkmanIndexDto?> GetBrinkmanIndexAsync(int accountId);
        
        /// <summary>
        /// Lấy thống kê Brinkman Index tổng quan của hệ thống
        /// </summary>
        Task<BrinkmanStatisticsDto> GetBrinkmanStatisticsAsync();
    }
} 