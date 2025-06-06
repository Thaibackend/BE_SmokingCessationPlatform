using SmokingQuitSupportAPI.Models.Entities;

namespace SmokingQuitSupportAPI.Data.Repositories.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
    }
} 