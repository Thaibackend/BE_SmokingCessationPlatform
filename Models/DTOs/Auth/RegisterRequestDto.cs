using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? FullName { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
    }
} 