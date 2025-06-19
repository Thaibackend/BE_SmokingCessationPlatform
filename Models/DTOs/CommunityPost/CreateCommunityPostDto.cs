using System.ComponentModel.DataAnnotations;

namespace SmokingQuitSupportAPI.Models.DTOs.CommunityPost
{
    /// <summary>
    /// DTO để tạo bài viết cộng đồng mới
    /// </summary>
    public class CreateCommunityPostDto
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string Content { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "Danh mục không được quá 50 ký tự")]
        public string? Category { get; set; }
    }
} 