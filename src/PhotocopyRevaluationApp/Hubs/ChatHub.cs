using Microsoft.AspNetCore.SignalR;
using PhotocopyRevaluationApp.ChatBoat;

namespace PhotocopyRevaluationApp.Hubs {
    public class ChatHub : Hub {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService) {
            _chatService = chatService;
        }

        // Send message to a specific group
        public async Task SendMessageToGroup(string groupName, string user, string message) {
            var timestamp = DateTime.Now;
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message, timestamp);

            // Save the message to the database
            await _chatService.SaveMessageAsync(groupName, user, message, timestamp);
        }

        // Track when a user starts typing in a group chat
        public async Task Typing(string groupName, string user) {
            await Clients.Group(groupName).SendAsync("UserTyping", user);
        }

        // Mark message as read by user
        public async Task MarkAsRead(string groupName, string messageId, string user) {
            //await _chatService.MarkMessageAsReadAsync(messageId, user);
            await Clients.Group(groupName).SendAsync("MessageRead", user, messageId);
        }

        // Join a group (e.g., chat room)
        public async Task JoinGroup(string groupName) {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        // Leave a group
        public async Task LeaveGroup(string groupName) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public override async Task OnConnectedAsync() {
            // Check if User or Identity is null before accessing the Name
            var userName = this.Context.User?.Identity?.Name ?? "Anonymous";

            // Broadcast user connected
            await Clients.All.SendAsync("UserConnected", userName);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            // Broadcast user disconnected
            // Check if User or Identity is null before accessing the Name
            var userName = this.Context.User?.Identity?.Name ?? "Anonymous";

            // Broadcast user connected
            await Clients.All.SendAsync("UserDisconnected", userName);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
