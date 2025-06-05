using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.DTOs.Auth
{
    public class RegisterUserRequestDto
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? FullName { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
    }
} 