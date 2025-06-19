using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data;
using SmokingQuitSupportAPI.Models.DTOs.CommunityPost;
using SmokingQuitSupportAPI.Models.Entities;
using SmokingQuitSupportAPI.Services.Interfaces;

namespace SmokingQuitSupportAPI.Services
{
    /// <summary>
    /// Service implementation cho bài viết cộng đồng - Blog chia sẻ kinh nghiệm
    /// </summary>
    public class CommunityPostService : ICommunityPostService
    {
        private readonly AppDbContext _context;

        public CommunityPostService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CommunityPostDto>> GetAllPostsAsync()
        {
            var posts = await _context.CommunityPosts
                .Include(p => p.Account)
                .Where(p => p.Status == "Published")
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new CommunityPostDto
                {
                    PostId = p.PostId,
                    AccountId = p.AccountId,
                    Title = p.Title,
                    Content = p.Content,
                    Category = p.Category,
                    Status = p.Status,
                    ViewCount = p.ViewCount,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    AuthorName = p.Account.FullName,
                    AuthorUsername = p.Account.Username,
                    LikesCount = p.PostLikes.Count,
                    CommentsCount = p.Comments.Count
                })
                .ToListAsync();

            return posts;
        }

        public async Task<CommunityPostDto?> GetPostByIdAsync(int postId)
        {
            var post = await _context.CommunityPosts
                .Include(p => p.Account)
                .Include(p => p.PostLikes)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null) return null;

            return new CommunityPostDto
            {
                PostId = post.PostId,
                AccountId = post.AccountId,
                Title = post.Title,
                Content = post.Content,
                Category = post.Category,
                Status = post.Status,
                ViewCount = post.ViewCount,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                AuthorName = post.Account.FullName,
                AuthorUsername = post.Account.Username,
                LikesCount = post.PostLikes.Count,
                CommentsCount = post.Comments.Count
            };
        }

        public async Task<IEnumerable<CommunityPostDto>> GetPostsByAuthorAsync(int accountId)
        {
            var posts = await _context.CommunityPosts
                .Include(p => p.Account)
                .Where(p => p.AccountId == accountId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new CommunityPostDto
                {
                    PostId = p.PostId,
                    AccountId = p.AccountId,
                    Title = p.Title,
                    Content = p.Content,
                    Category = p.Category,
                    Status = p.Status,
                    ViewCount = p.ViewCount,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    AuthorName = p.Account.FullName,
                    AuthorUsername = p.Account.Username,
                    LikesCount = p.PostLikes.Count,
                    CommentsCount = p.Comments.Count
                })
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<CommunityPostDto>> GetPostsByCategoryAsync(string category)
        {
            var posts = await _context.CommunityPosts
                .Include(p => p.Account)
                .Where(p => p.Category == category && p.Status == "Published")
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new CommunityPostDto
                {
                    PostId = p.PostId,
                    AccountId = p.AccountId,
                    Title = p.Title,
                    Content = p.Content,
                    Category = p.Category,
                    Status = p.Status,
                    ViewCount = p.ViewCount,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    AuthorName = p.Account.FullName,
                    AuthorUsername = p.Account.Username,
                    LikesCount = p.PostLikes.Count,
                    CommentsCount = p.Comments.Count
                })
                .ToListAsync();

            return posts;
        }

        public async Task<CommunityPostDto> CreatePostAsync(int accountId, CreateCommunityPostDto createPostDto)
        {
            var post = new CommunityPost
            {
                AccountId = accountId,
                Title = createPostDto.Title,
                Content = createPostDto.Content,
                Category = createPostDto.Category,
                Status = "Published",
                ViewCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CommunityPosts.Add(post);
            await _context.SaveChangesAsync();

            return await GetPostByIdAsync(post.PostId) ?? throw new InvalidOperationException("Failed to create post");
        }

        public async Task<CommunityPostDto?> UpdatePostAsync(int postId, int accountId, CreateCommunityPostDto updatePostDto)
        {
            var post = await _context.CommunityPosts
                .FirstOrDefaultAsync(p => p.PostId == postId && p.AccountId == accountId);

            if (post == null) return null;

            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;
            post.Category = updatePostDto.Category;
            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetPostByIdAsync(postId);
        }

        public async Task<bool> DeletePostAsync(int postId, int accountId)
        {
            var post = await _context.CommunityPosts
                .FirstOrDefaultAsync(p => p.PostId == postId && p.AccountId == accountId);

            if (post == null) return false;

            _context.CommunityPosts.Remove(post);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleLikeAsync(int postId, int accountId)
        {
            var existingLike = await _context.PostLikes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.AccountId == accountId);

            if (existingLike != null)
            {
                _context.PostLikes.Remove(existingLike);
                await _context.SaveChangesAsync();
                return false;
            }
            else
            {
                var like = new PostLike
                {
                    PostId = postId,
                    AccountId = accountId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PostLikes.Add(like);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task IncrementViewCountAsync(int postId)
        {
            var post = await _context.CommunityPosts.FindAsync(postId);
            if (post != null)
            {
                post.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CommunityPostDto>> GetPopularPostsAsync(int take = 10)
        {
            var posts = await _context.CommunityPosts
                .Include(p => p.Account)
                .Where(p => p.Status == "Published")
                .OrderByDescending(p => p.ViewCount)
                .ThenByDescending(p => p.PostLikes.Count)
                .Take(take)
                .Select(p => new CommunityPostDto
                {
                    PostId = p.PostId,
                    AccountId = p.AccountId,
                    Title = p.Title,
                    Content = p.Content,
                    Category = p.Category,
                    Status = p.Status,
                    ViewCount = p.ViewCount,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    AuthorName = p.Account.FullName,
                    AuthorUsername = p.Account.Username,
                    LikesCount = p.PostLikes.Count,
                    CommentsCount = p.Comments.Count
                })
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<CommunityPostDto>> SearchPostsAsync(string searchTerm)
        {
            var posts = await _context.CommunityPosts
                .Include(p => p.Account)
                .Where(p => p.Status == "Published" && 
                           (p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm)))
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new CommunityPostDto
                {
                    PostId = p.PostId,
                    AccountId = p.AccountId,
                    Title = p.Title,
                    Content = p.Content,
                    Category = p.Category,
                    Status = p.Status,
                    ViewCount = p.ViewCount,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    AuthorName = p.Account.FullName,
                    AuthorUsername = p.Account.Username,
                    LikesCount = p.PostLikes.Count,
                    CommentsCount = p.Comments.Count
                })
                .ToListAsync();

            return posts;
        }
    }
} 