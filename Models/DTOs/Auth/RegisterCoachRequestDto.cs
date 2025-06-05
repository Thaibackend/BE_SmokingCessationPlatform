using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.DTOs.Auth
{
    public class RegisterCoachRequestDto
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
        
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        
        // Thông tin chuyên môn cho coach
        [Required(ErrorMessage = "Bằng cấp là bắt buộc")]
        [StringLength(500)]
        public string Qualifications { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Kinh nghiệm là bắt buộc")]
        [StringLength(1000)]
        public string Experience { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Specialization { get; set; }
    }
} 