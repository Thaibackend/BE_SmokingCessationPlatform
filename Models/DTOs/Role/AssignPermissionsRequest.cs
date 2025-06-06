using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.DTOs.Role
{
    public class AssignPermissionsRequest
    {
        [Required]
        public List<int> PermissionIds { get; set; }
    }
} 