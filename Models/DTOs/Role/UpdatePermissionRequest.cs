using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.DTOs.Role
{
    public class UpdatePermissionRequest
    {
        [StringLength(200)]
        public string Description { get; set; }
    }
} 