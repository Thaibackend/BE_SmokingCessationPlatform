using System;

namespace SmokingQuitSupportAPI.Models.DTOs.Role
{
    public class RolePermissionDto
    {
        public int RolePermissionId { get; set; }
        
        public int RoleId { get; set; }
        
        public int PermissionId { get; set; }
        
        public string RoleName { get; set; }
        
        public string PermissionName { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
} 