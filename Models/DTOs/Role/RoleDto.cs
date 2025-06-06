using System;
using System.Collections.Generic;

namespace SmokingQuitSupportAPI.Models.DTOs.Role
{
    public class RoleDto
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<PermissionDto> Permissions { get; set; }
    }
} 