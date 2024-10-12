using Microsoft.AspNetCore.SignalR;
using PhotocopyRevaluationAppMVC.Services;

namespace PhotocopyRevaluationAppMVC.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;

        public NotificationHub(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // Send notification to a specific user
        public async Task SendNotification(string userId, string message, string priority)
        {
            // Based on priority, you could handle different types of notifications
            if (priority == "high")
            {
                await Clients.User(userId).SendAsync("ReceiveHighPriorityNotification", message);
            }
            else
            {
                await Clients.User(userId).SendAsync("ReceiveNotification", message);
            }

            // Log or save the notification to the database
            //await _notificationService.SaveNotificationAsync(userId, message, priority);
        }

        // Broadcast a system-wide notification
        public async Task BroadcastNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveBroadcastNotification", message);
        }

        // Override when a connection is established
        public override async Task OnConnectedAsync()
        {
            // Notify admin that a user connected (could be useful for tracking online users)
            await Clients.Group("Admins").SendAsync("UserConnected", Context.UserIdentifier);
            await base.OnConnectedAsync();
        }

        // Override when a connection is disconnected
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Notify admin that a user disconnected
            await Clients.Group("Admins").SendAsync("UserDisconnected", Context.UserIdentifier);
            await base.OnDisconnectedAsync(exception);
        }
    }
}