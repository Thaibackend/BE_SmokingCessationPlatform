using SmokingQuitSupportAPI.Models.DTOs.Account;
using SmokingQuitSupportAPI.Models.DTOs.Auth;

namespace SmokingQuitSupportAPI.Services.Interfaces
{
    /// <summary>
    /// Interface định nghĩa các phương thức xác thực người dùng
    /// Bao gồm đăng ký, đăng nhập và tạo JWT token
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        /// <param name="createAccountDto">Thông tin tài khoản cần tạo</param>
        /// <returns>Thông tin tài khoản đã tạo</returns>
        Task<AccountDto> RegisterAsync(CreateAccountDto createAccountDto);
        
        /// <summary>
        /// Đăng nhập và trả về JWT token
        /// </summary>
        /// <param name="loginDto">Thông tin đăng nhập</param>
        /// <returns>JWT token nếu đăng nhập thành công</returns>
        Task<string> LoginAsync(LoginDto loginDto);
        
        /// <summary>
        /// Lấy thông tin tài khoản theo ID
        /// </summary>
        /// <param name="accountId">ID của tài khoản</param>
        /// <returns>Thông tin tài khoản</returns>
        Task<AccountDto?> GetAccountByIdAsync(int accountId);
    }
} 