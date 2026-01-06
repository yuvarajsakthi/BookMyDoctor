using AutoMapper;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Repositories.UnitOfWork;
using BookMyDoctor.Server.Services.Interfaces;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task SendNotificationAsync(int userId, string message, string? title = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                SentAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _unitOfWork.Notifications.FindAsync(n => n.UserId == userId);
            return notifications.OrderByDescending(n => n.SentAt)
                              .Select(n => _mapper.Map<NotificationResponseDto>(n));
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            await _unitOfWork.Notifications.DeleteByIdAsync(notificationId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}