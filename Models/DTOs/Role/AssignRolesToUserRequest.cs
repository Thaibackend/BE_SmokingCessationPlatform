using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.DTOs.Role
{
    public class AssignRolesToUserRequest
    {
        [Required]
        public List<int> RoleIds { get; set; }
    }
} 