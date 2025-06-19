namespace SmokingQuitSupportAPI.Models.DTOs.CommunityPost
{
    /// <summary>
    /// DTO cho bài viết cộng đồng - Blog chia sẻ kinh nghiệm
    /// </summary>
    public class CommunityPostDto
    {
        public int PostId { get; set; }
        public int AccountId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Thông tin tác giả
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorUsername { get; set; } = string.Empty;
        
        // Thống kê tương tác
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }
} 