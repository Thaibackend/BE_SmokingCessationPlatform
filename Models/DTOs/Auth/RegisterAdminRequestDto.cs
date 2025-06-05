using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.DTOs.Auth
{
    public class RegisterAdminRequestDto
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(8, ErrorMessage = "Mật khẩu admin phải có ít nhất 8 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Mã xác thực admin là bắt buộc")]
        public string AdminCode { get; set; } = string.Empty;
    }
} 