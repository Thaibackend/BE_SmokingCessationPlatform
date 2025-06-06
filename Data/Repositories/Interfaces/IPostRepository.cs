using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories.Interfaces
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<IEnumerable<Post>> GetByUserIdAsync(int userId);
        new Task<Post> UpdateAsync(Post post);
        Task DeleteAsync(int id);
        Task<bool> AddInteractionAsync(PostInteraction interaction);
        Task<bool> RemoveInteractionAsync(int userId, int postId, string type);
        Task<IEnumerable<Post>> GetCommentsByPostIdAsync(int postId);
    }
} 