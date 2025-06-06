using Microsoft.EntityFrameworkCore;
using SmokingQuitSupportAPI.Data.Repositories.Interfaces;
using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _context.Posts
                .Where(p => p.Type == "Post")
                .Include(p => p.User)
                .Include(p => p.Comments.Where(c => c.Type == "Comment"))
                .ThenInclude(c => c.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetByUserIdAsync(int userId)
        {
            return await _context.Posts
                .Where(p => p.UserId == userId && p.Type == "Post")
                .Include(p => p.User)
                .Include(p => p.Comments.Where(c => c.Type == "Comment"))
                .ThenInclude(c => c.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Post> UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task DeleteAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> AddInteractionAsync(PostInteraction interaction)
        {
            try
            {
                var exists = await _context.PostInteractions
                    .AnyAsync(i => i.PostId == interaction.PostId && 
                                  i.UserId == interaction.UserId &&
                                  i.Type == interaction.Type);

                if (exists) return false;

                _context.PostInteractions.Add(interaction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveInteractionAsync(int userId, int postId, string type)
        {
            var interaction = await _context.PostInteractions
                .FirstOrDefaultAsync(i => i.PostId == postId && 
                                        i.UserId == userId &&
                                        i.Type == type);

            if (interaction == null) return false;

            _context.PostInteractions.Remove(interaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PostInteraction?> GetInteractionAsync(int postId, int userId, string type)
        {
            return await _context.PostInteractions
                .FirstOrDefaultAsync(i => i.PostId == postId && i.UserId == userId && i.Type == type);
        }

        public async Task IncrementLikeCountAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                post.LikeCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DecrementLikeCountAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null && post.LikeCount > 0)
            {
                post.LikeCount--;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Post>> GetCommentsByPostIdAsync(int postId)
        {
            return await _context.Posts
                .Where(p => p.ParentId == postId && p.Type == "Comment")
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
} 