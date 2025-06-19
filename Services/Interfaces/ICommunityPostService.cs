using SmokingQuitSupportAPI.Models.DTOs.CommunityPost;

namespace SmokingQuitSupportAPI.Services.Interfaces
{
    /// <summary>
    /// Interface cho dịch vụ bài viết cộng đồng - Blog chia sẻ kinh nghiệm
    /// </summary>
    public interface ICommunityPostService
    {
        /// <summary>
        /// Lấy danh sách tất cả bài viết cộng đồng
        /// </summary>
        Task<IEnumerable<CommunityPostDto>> GetAllPostsAsync();
        
        /// <summary>
        /// Lấy bài viết theo ID
        /// </summary>
        Task<CommunityPostDto?> GetPostByIdAsync(int postId);
        
        /// <summary>
        /// Lấy bài viết theo tác giả
        /// </summary>
        Task<IEnumerable<CommunityPostDto>> GetPostsByAuthorAsync(int accountId);
        
        /// <summary>
        /// Lấy bài viết theo danh mục
        /// </summary>
        Task<IEnumerable<CommunityPostDto>> GetPostsByCategoryAsync(string category);
        
        /// <summary>
        /// Tạo bài viết mới
        /// </summary>
        Task<CommunityPostDto> CreatePostAsync(int accountId, CreateCommunityPostDto createPostDto);
        
        /// <summary>
        /// Cập nhật bài viết
        /// </summary>
        Task<CommunityPostDto?> UpdatePostAsync(int postId, int accountId, CreateCommunityPostDto updatePostDto);
        
        /// <summary>
        /// Xóa bài viết
        /// </summary>
        Task<bool> DeletePostAsync(int postId, int accountId);
        
        /// <summary>
        /// Like/Unlike bài viết
        /// </summary>
        Task<bool> ToggleLikeAsync(int postId, int accountId);
        
        /// <summary>
        /// Tăng lượt xem bài viết
        /// </summary>
        Task IncrementViewCountAsync(int postId);
        
        /// <summary>
        /// Lấy bài viết phổ biến nhất
        /// </summary>
        Task<IEnumerable<CommunityPostDto>> GetPopularPostsAsync(int take = 10);
        
        /// <summary>
        /// Tìm kiếm bài viết
        /// </summary>
        Task<IEnumerable<CommunityPostDto>> SearchPostsAsync(string searchTerm);
    }
} 