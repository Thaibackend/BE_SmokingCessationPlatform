using SmokingQuitSupportAPI.Models.DTOs.Package;

namespace SmokingQuitSupportAPI.Services.Interfaces
{
    /// <summary>
    /// Interface định nghĩa các operations cho Package Service
    /// </summary>
    public interface IPackageService
    {
        /// <summary>
        /// Tạo subscription Basic mặc định khi user đăng ký
        /// </summary>
        /// <param name="accountId">ID của account</param>
        /// <returns>Thông tin package được tạo</returns>
        Task<UserPackageDto> CreateBasicSubscriptionAsync(int accountId);

        /// <summary>
        /// Lấy thông tin package hiện tại của user
        /// </summary>
        /// <param name="accountId">ID của account</param>
        /// <returns>Thông tin package hiện tại hoặc null</returns>
        Task<UserPackageDto?> GetUserPackageAsync(int accountId);

        /// <summary>
        /// Upgrade package lên gói mới (thường là Premium)
        /// </summary>
        /// <param name="accountId">ID của account</param>
        /// <param name="upgradeDto">Thông tin upgrade</param>
        /// <returns>Thông tin package sau khi upgrade</returns>
        Task<UserPackageDto> UpgradeToPackageAsync(int accountId, UpgradePackageDto upgradeDto);

        /// <summary>
        /// Tự động đề xuất quit plan cho Basic users
        /// </summary>
        /// <param name="accountId">ID của account</param>
        /// <returns>Kế hoạch cai thuốc được đề xuất</returns>
        Task<SuggestedQuitPlanDto> GenerateSuggestedQuitPlanAsync(int accountId);

        /// <summary>
        /// Kiểm tra user có quyền truy cập Premium features không
        /// </summary>
        /// <param name="accountId">ID của account</param>
        /// <returns>True nếu có quyền truy cập Premium</returns>
        Task<bool> HasPremiumAccessAsync(int accountId);
    }
} 