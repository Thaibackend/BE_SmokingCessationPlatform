using System;

namespace SmokingQuitSupportAPI.Models.DTOs.Role
{
    public class UserRoleDto
    {
        public int UserRoleId { get; set; }
        
        public int UserId { get; set; }
        
        public int RoleId { get; set; }
        
        public string RoleName { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
} 