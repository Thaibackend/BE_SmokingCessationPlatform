using System;
using System.ComponentModel.DataAnnotations;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class PostInteraction
    {
        [Key]
        public int InteractionId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } = string.Empty; // Like, Save, Report
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Post Post { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
} 