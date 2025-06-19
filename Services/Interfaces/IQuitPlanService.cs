using SmokingQuitSupportAPI.Models.DTOs.QuitPlan;

namespace SmokingQuitSupportAPI.Services.Interfaces
{
    /// <summary>
    /// Interface cho dịch vụ kế hoạch cai thuốc
    /// </summary>
    public interface IQuitPlanService
    {
        /// <summary>
        /// Lấy tất cả kế hoạch cai thuốc của người dùng
        /// </summary>
        Task<IEnumerable<QuitPlanDto>> GetUserQuitPlansAsync(int accountId);
        
        /// <summary>
        /// Lấy kế hoạch cai thuốc theo ID
        /// </summary>
        Task<QuitPlanDto?> GetQuitPlanByIdAsync(int planId);
        
        /// <summary>
        /// Lấy kế hoạch cai thuốc đang hoạt động của người dùng
        /// </summary>
        Task<QuitPlanDto?> GetActiveQuitPlanAsync(int accountId);
        
        /// <summary>
        /// Tạo kế hoạch cai thuốc mới
        /// </summary>
        Task<QuitPlanDto> CreateQuitPlanAsync(int accountId, CreateQuitPlanDto createPlanDto);
        
        /// <summary>
        /// Cập nhật kế hoạch cai thuốc
        /// </summary>
        Task<QuitPlanDto?> UpdateQuitPlanAsync(int planId, int accountId, CreateQuitPlanDto updatePlanDto);
        
        /// <summary>
        /// Xóa kế hoạch cai thuốc
        /// </summary>
        Task<bool> DeleteQuitPlanAsync(int planId, int accountId);
        
        /// <summary>
        /// Tạo kế hoạch cai thuốc tự động dựa trên thông tin người dùng
        /// </summary>
        Task<QuitPlanDto> GenerateAutomaticQuitPlanAsync(int accountId, int packageId);
        
        /// <summary>
        /// Cập nhật trạng thái kế hoạch
        /// </summary>
        Task<bool> UpdatePlanStatusAsync(int planId, string status);
        
        /// <summary>
        /// Lấy thống kê kế hoạch cai thuốc
        /// </summary>
        Task<object> GetQuitPlanStatisticsAsync(int accountId);
    }
} 