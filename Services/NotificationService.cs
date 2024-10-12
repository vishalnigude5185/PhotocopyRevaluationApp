using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PhotocopyRevaluationAppMVC.Data;
using PhotocopyRevaluationAppMVC.Enums;
using PhotocopyRevaluationAppMVC.Hubs;
using PhotocopyRevaluationAppMVC.Models;
using System.Timers;

namespace PhotocopyRevaluationAppMVC.Services
{
    public class NotificationService : INotificationService
    {
        private readonly PhotocopyRevaluationAppContext _context; // Assuming you have an EF Core context

        private readonly IUserConnectionManager _connectionManager;
        private readonly IUserSessionService _userSessionService;
        private readonly IHubContext<SignOutHub> _signOutHubContext;

        private bool isTimerRunning = false; // Class-level variable to track the timer state
        private System.Timers.Timer sessionCheckTimer;

        public NotificationService(PhotocopyRevaluationAppContext context, IHubContext<SignOutHub> signOutHubContext, IUserConnectionManager connectionManager, IUserSessionService userSessionService)
        {
            _context = context;
            _signOutHubContext = signOutHubContext;
            _connectionManager = connectionManager;
            _userSessionService = userSessionService;
        }

        //========================== Current Required Code Start ================================

        // This method will be called when the timer elapses (every minute)
        public async Task CheackAndSendSignOutNotificationByUserIdThreeMinutesBeforeAsync()
        {
            string message = "Please note: Your session is going to expire within five minuits.";
            // check session expirations
            // Check for session expirations immediately when called
            foreach (var sessionData in _userSessionService.GetAllUsersSessionDataAsync().Values) // Use ToList to avoid modifying collection during iteration
            {
                if (sessionData.ExpiryTime <= DateTime.Now.AddMinutes(1))
                {
                    string userId = sessionData.ApplicationUserId.ToString();
                    // Broadcast sign-out to specific user for connected with all connections
                    // This method can be invoked from the client-side to sign out users
                    await _signOutHubContext.Clients.User(userId).SendAsync("SendSignOutNotificationByUserIdThreeMinutesBeforeAsync", message);
                }
            }
        }
        // This method will be called when the timer elapses (every minute)
        public async Task CheckAndSendSignOutNotificationByUserIdThreeSecondsBeforeAsync()
        {
            string message = "Your session will expire soon.";
            // Add logic to check session expirations
            // Check for session expirations immediately when called
            foreach (var sessionData in _userSessionService.GetAllUsersSessionDataAsync().Values) // Use ToList to avoid modifying collection during iteration
            {
                if (sessionData.ExpiryTime <= DateTime.Now)
                {
                    string userId = sessionData.ApplicationUserId.ToString();
                    // Broadcast sign-out to specific user or all connected users
                    // This method can be invoked from the client-side to sign out users
                    _signOutHubContext.Clients.User(userId).SendAsync("SendSignOutNotificationByUserIdThreeSecondsBeforeAsync", message);
                }

            }
        }
        //================================================ Current Required Code End =============================================
        public async Task<List<Notification>> GetSignOutNotificationsForUserAsync(string userId)
        {
            // Fetch notifications where ApplicationUserId matches and IsRead is false (meaning ReadAt is null)
            var notifications = await _context.Notifications
                .Where(n => n.ApplicationUserId == Convert.ToInt32(userId) && n.ReadAt == null) // ReadAt == null means it's unread
                .OrderByDescending(n => n.CreatedAt) // Order by the most recent notifications first
                .ToListAsync();

            return notifications;
        }
        public async Task CreateNotificationAsync(string userId, string message, NotificationType type, NotificationPriority priority, string actionUrl = null)
        {
            var notification = new Notification
            {
                ApplicationUserId = Convert.ToInt32(userId),
                Message = message,
                Type = type,
                Priority = priority,
                ActionUrl = actionUrl,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7) // Expire the notification after 7 days, for example
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
        public async Task SendSignOutNotificationToUserByConnectionIdAsync(string userId)
        {
            // Get all connection IDs for the user
            var connectionIds = await _connectionManager.GetConnectionIdsAsync(userId);

            foreach (var connectionId in connectionIds)
            {
                // Notify each connection to sign out
                await _signOutHubContext.Clients.Client(connectionId).SendAsync("SignOut");
            }
        }
        public async Task MarkAsRead(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.ReadAt = DateTime.UtcNow; // Marking the notification as read
                                                       //notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
