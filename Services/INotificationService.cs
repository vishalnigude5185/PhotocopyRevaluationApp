using PhotocopyRevaluationAppMVC.Enums;
using PhotocopyRevaluationAppMVC.Models;
using System.Timers;

namespace PhotocopyRevaluationAppMVC.Services
{
    public interface INotificationService
    {
        Task CheackAndSendSignOutNotificationByUserIdThreeMinutesBeforeAsync();
        Task CheckAndSendSignOutNotificationByUserIdThreeSecondsBeforeAsync();
        Task CreateNotificationAsync(string userId, string message, NotificationType type, NotificationPriority priority, string actionUrl = null);
        Task<List<Notification>> GetSignOutNotificationsForUserAsync(string userId);
        Task MarkAsRead(int notificationId);
        Task SendSignOutNotificationToUserByConnectionIdAsync(string userId);
    }
}