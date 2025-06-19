using System.ComponentModel.DataAnnotations;
using SmokingQuitSupportAPI.Constants;

namespace SmokingQuitSupportAPI.Models.DTOs.Account
{
    public class CreateAccountDto
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
        public string Password { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? FullName { get; set; }
        
        [StringLength(20)]
        public string Role { get; set; } = Roles.User;

        /// <summary>
        /// Validate role value
        /// </summary>
        public bool IsValidRole => Roles.IsValidRole(Role);
    }
} 