using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmokingQuitSupportAPI.Models.Entities
{
    public class ChatMessage
    {
        [Key]
        public int MessageId { get; set; }
        
        [Required]
        public int SenderId { get; set; }
        
        [Required]
        public int ReceiverId { get; set; }
        
        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? MessageType { get; set; } = "TEXT"; // TEXT, IMAGE, FILE
        
        [StringLength(500)]
        public string? AttachmentUrl { get; set; }
        
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        
        // Navigation Properties
        public virtual Account Sender { get; set; } = null!;
        public virtual Account Receiver { get; set; } = null!;
    }
} 