using System;

namespace SmokingQuitSupportAPI.Models.DTOs.Role
{
    public class PermissionDto
    {
        public int PermissionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 