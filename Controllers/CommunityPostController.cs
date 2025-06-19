using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmokingQuitSupportAPI.Models.DTOs.CommunityPost;
using SmokingQuitSupportAPI.Services.Interfaces;
using System.Security.Claims;

namespace SmokingQuitSupportAPI.Controllers
{
    /// <summary>
    /// Controller cho bài viết cộng đồng - Blog chia sẻ kinh nghiệm
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CommunityPostController : ControllerBase
    {
        private readonly ICommunityPostService _communityPostService;

        public CommunityPostController(ICommunityPostService communityPostService)
        {
            _communityPostService = communityPostService;
        }

        /// <summary>
        /// Lấy tất cả bài viết cộng đồng
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommunityPostDto>>> GetAllPosts()
        {
            try
            {
                var posts = await _communityPostService.GetAllPostsAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy bài viết theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CommunityPostDto>> GetPostById(int id)
        {
            try
            {
                var post = await _communityPostService.GetPostByIdAsync(id);
                if (post == null)
                {
                    return NotFound(new { message = "Không tìm thấy bài viết" });
                }

                // Tăng lượt xem
                await _communityPostService.IncrementViewCountAsync(id);
                
                return Ok(post);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy bài viết theo danh mục
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<CommunityPostDto>>> GetPostsByCategory(string category)
        {
            try
            {
                var posts = await _communityPostService.GetPostsByCategoryAsync(category);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy bài viết theo danh mục", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy bài viết phổ biến
        /// </summary>
        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<CommunityPostDto>>> GetPopularPosts([FromQuery] int take = 10)
        {
            try
            {
                var posts = await _communityPostService.GetPopularPostsAsync(take);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy bài viết phổ biến", error = ex.Message });
            }
        }

        /// <summary>
        /// Tìm kiếm bài viết
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CommunityPostDto>>> SearchPosts([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Từ khóa tìm kiếm không được để trống" });
                }

                var posts = await _communityPostService.SearchPostsAsync(searchTerm);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tìm kiếm bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy bài viết của tôi
        /// </summary>
        [HttpGet("my-posts")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CommunityPostDto>>> GetMyPosts()
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var posts = await _communityPostService.GetPostsByAuthorAsync(accountId);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy bài viết của bạn", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo bài viết mới
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommunityPostDto>> CreatePost([FromBody] CreateCommunityPostDto createPostDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var post = await _communityPostService.CreatePostAsync(accountId, createPostDto);
                
                return CreatedAtAction(nameof(GetPostById), new { id = post.PostId }, post);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật bài viết
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<CommunityPostDto>> UpdatePost(int id, [FromBody] CreateCommunityPostDto updatePostDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var post = await _communityPostService.UpdatePostAsync(id, accountId, updatePostDto);
                
                if (post == null)
                {
                    return NotFound(new { message = "Không tìm thấy bài viết hoặc bạn không có quyền chỉnh sửa" });
                }

                return Ok(post);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa bài viết
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeletePost(int id)
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var success = await _communityPostService.DeletePostAsync(id, accountId);
                
                if (!success)
                {
                    return NotFound(new { message = "Không tìm thấy bài viết hoặc bạn không có quyền xóa" });
                }

                return Ok(new { message = "Xóa bài viết thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa bài viết", error = ex.Message });
            }
        }

        /// <summary>
        /// Like/Unlike bài viết
        /// </summary>
        [HttpPost("{id}/toggle-like")]
        [Authorize]
        public async Task<ActionResult> ToggleLike(int id)
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isLiked = await _communityPostService.ToggleLikeAsync(id, accountId);
                
                return Ok(new { 
                    message = isLiked ? "Đã thích bài viết" : "Đã bỏ thích bài viết",
                    isLiked = isLiked
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi thực hiện like/unlike", error = ex.Message });
            }
        }
    }
} 