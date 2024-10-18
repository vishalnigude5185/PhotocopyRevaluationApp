using PhotocopyRevaluationApp.Enums;
using PhotocopyRevaluationApp.Models;

namespace PhotocopyRevaluationApp.Services {
    public interface INotificationService {
        Task CheackAndSendSignOutNotificationByUserIdThreeMinutesBeforeAsync();
        Task CheckAndSendSignOutNotificationByUserIdThreeSecondsBeforeAsync();
        Task CreateNotificationAsync(string userId, string message, NotificationType type, NotificationPriority priority, string actionUrl = null);
        Task<List<Notification>> GetSignOutNotificationsForUserAsync(string userId);
        Task MarkAsRead(int notificationId);
        Task SendSignOutNotificationToUserByConnectionIdAsync(string userId);
    }
}