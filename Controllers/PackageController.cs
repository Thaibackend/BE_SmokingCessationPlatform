using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.Package;
using SmokingQuitSupportAPI.Services.Interfaces;
using System.Security.Claims;

namespace SmokingQuitSupportAPI.Controllers
{
    /// <summary>
    /// Controller xử lý các operations liên quan đến Package Management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        /// <summary>
        /// Lấy thông tin gói dịch vụ hiện tại của user
        /// </summary>
        /// <returns>Thông tin package hiện tại</returns>
        [HttpGet("my-package")]
        public async Task<ActionResult<UserPackageDto>> GetMyPackage()
        {
            try
            {
                var accountId = GetCurrentAccountId();
                var package = await _packageService.GetUserPackageAsync(accountId);

                if (package == null)
                {
                    // Tự động tạo gói Basic nếu chưa có
                    package = await _packageService.CreateBasicSubscriptionAsync(accountId);
                }

                return Ok(package);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Upgrade lên gói Premium
        /// </summary>
        /// <param name="upgradeDto">Thông tin upgrade</param>
        /// <returns>Thông tin package sau khi upgrade</returns>
        [HttpPost("upgrade")]
        public async Task<ActionResult<UserPackageDto>> UpgradePackage([FromBody] UpgradePackageDto upgradeDto)
        {
            try
            {
                var accountId = GetCurrentAccountId();
                upgradeDto.AccountId = accountId;

                var upgradedPackage = await _packageService.UpgradeToPackageAsync(accountId, upgradeDto);
                
                return Ok(new
                {
                    message = "Upgrade package thành công!",
                    package = upgradedPackage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy đề xuất kế hoạch cai thuốc tự động (cho Basic users)
        /// </summary>
        /// <returns>Kế hoạch cai thuốc được đề xuất</returns>
        [HttpGet("suggested-quit-plan")]
        public async Task<ActionResult<SuggestedQuitPlanDto>> GetSuggestedQuitPlan()
        {
            try
            {
                var accountId = GetCurrentAccountId();
                var suggestedPlan = await _packageService.GenerateSuggestedQuitPlanAsync(accountId);
                
                return Ok(suggestedPlan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra quyền truy cập Premium features
        /// </summary>
        /// <returns>Thông tin về quyền truy cập Premium</returns>
        [HttpGet("premium-access")]
        public async Task<ActionResult<object>> CheckPremiumAccess()
        {
            try
            {
                var accountId = GetCurrentAccountId();
                var hasPremiumAccess = await _packageService.HasPremiumAccessAsync(accountId);
                
                return Ok(new
                {
                    hasPremiumAccess = hasPremiumAccess,
                    message = hasPremiumAccess 
                        ? "Bạn có quyền truy cập tất cả tính năng Premium" 
                        : "Upgrade lên Premium để sử dụng đầy đủ tính năng"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách tính năng có sẵn theo gói
        /// </summary>
        /// <returns>Danh sách tính năng</returns>
        [HttpGet("available-features")]
        public async Task<ActionResult<object>> GetAvailableFeatures()
        {
            try
            {
                var accountId = GetCurrentAccountId();
                var package = await _packageService.GetUserPackageAsync(accountId);

                if (package == null)
                {
                    return Ok(new
                    {
                        packageType = "NONE",
                        features = new List<string>(),
                        message = "Chưa có gói dịch vụ nào được kích hoạt"
                    });
                }

                return Ok(new
                {
                    packageType = package.PackageType,
                    features = package.AvailableFeatures,
                    isPremium = package.IsPremium,
                    daysRemaining = package.DaysRemaining
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy AccountId từ JWT token
        /// </summary>
        private int GetCurrentAccountId()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                throw new UnauthorizedAccessException("Invalid token");
            }
            return accountId;
        }
    }
} 