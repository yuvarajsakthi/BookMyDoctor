using BookMyDoctor.Server.DTOs;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int userId, string message, string? title = null);
        Task<IEnumerable<NotificationResponseDto>> GetUserNotificationsAsync(int userId);
        Task DeleteNotificationAsync(int notificationId);
    }
}