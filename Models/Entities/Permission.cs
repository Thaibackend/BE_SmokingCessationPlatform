using System;
using System.Collections.Generic;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
} 