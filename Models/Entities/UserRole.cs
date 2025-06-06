using System;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Role Role { get; set; }
    }
} 