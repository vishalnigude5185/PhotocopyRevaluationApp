using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using PhotocopyRevaluationApp.Hubs;

namespace PhotocopyRevaluationApp.ChatBoat {
    public class ChatService : IChatService {
        private readonly IHubContext<ChatHub> _hubContext;

        // This will act as our in-memory storage for messages. In real-world scenarios, this would be persisted in a database.
        private static readonly ConcurrentDictionary<string, List<(string UserId, string Message, DateTime Timestamp)>> _groupMessages = new();
        private static readonly ConcurrentDictionary<string, List<(string SenderId, string ReceiverId, string Message, DateTime Timestamp)>> _privateMessages = new();

        // Active users tracking (for presence feature)
        private static readonly ConcurrentDictionary<string, string> _activeUsers = new();

        public ChatService(IHubContext<ChatHub> hubContext) {
            _hubContext = hubContext;
        }

        public async Task SaveMessageAsync(string groupName, string userId, string message, DateTime timestamp) {
            if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(message)) {
                throw new ArgumentException("Invalid message or user.");
            }

            // Assuming that you have an in-memory store for group messages (or a database)
            if (!_groupMessages.ContainsKey(groupName)) {
                //_groupMessages[groupName] = new List<(string, string, DateTime, bool)>();
            }

            // Save the message along with a 'read' status (default: false)
            //_groupMessages[groupName].Add((userId, message, timestamp, false));

            // Simulate async database call, replace this with actual DB logic
            await Task.CompletedTask;

            // Log or notify clients that the message was saved
            //_logger.LogInformation($"Message from {userId} in {groupName} saved at {timestamp}: {message}");
        }
        public async Task MarkMessageAsReadAsync(int messageId, string userId) {
            if (string.IsNullOrWhiteSpace(userId)) {
                throw new ArgumentException("User ID cannot be null or empty.");
            }

            // Locate the message by its ID in the in-memory store (or replace with database call)
            foreach (var group in _groupMessages.Values) {
                //var message = group.FirstOrDefault(m => m.Item4 == false && m.Item1 == userId); // For simplicity, assuming messageId is index or userId match

                //if (message != default)
                //{
                //    var updatedMessage = (message.UserId, message.Message, message.Timestamp, true);
                //    group.Remove(message);
                //    group.Add(updatedMessage); // Update the 'read' status
                //    break;
                //}
            }

            // Simulate async database operation
            await Task.CompletedTask;

            //_logger.LogInformation($"Message {messageId} marked as read by {userId}.");
        }
        // Sends a message to a group (room)
        public async Task<bool> SendMessageToGroupAsync(string groupName, string userId, string message) {
            if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(message)) {
                return false;
            }

            // Add message to in-memory storage
            await AddMessageToHistoryAsync(groupName, userId, message);

            // Broadcast the message to all clients in the group
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", userId, message, DateTime.UtcNow);
            return true;
        }

        // Sends a private message to a user
        public async Task<bool> SendMessageToUserAsync(string receiverUserId, string senderUserId, string message) {
            if (string.IsNullOrWhiteSpace(receiverUserId) || string.IsNullOrWhiteSpace(senderUserId) || string.IsNullOrWhiteSpace(message)) {
                return false;
            }

            // Add private message to in-memory storage
            if (!_privateMessages.ContainsKey(senderUserId)) {
                _privateMessages[senderUserId] = new List<(string, string, string, DateTime)>();
            }

            _privateMessages[senderUserId].Add((senderUserId, receiverUserId, message, DateTime.UtcNow));

            // Send the private message
            await _hubContext.Clients.User(receiverUserId).SendAsync("ReceivePrivateMessage", senderUserId, message, DateTime.UtcNow);
            return true;
        }

        // Adds a user to a group (room)
        public async Task JoinGroupAsync(string groupName, string userId) {
            await _hubContext.Groups.AddToGroupAsync(userId, groupName);

            if (!_groupMessages.ContainsKey(groupName)) {
                _groupMessages[groupName] = new List<(string, string, DateTime)>();
            }

            _activeUsers[userId] = groupName; // Track the group the user is in
        }

        // Removes a user from a group (room)
        public async Task LeaveGroupAsync(string groupName, string userId) {
            await _hubContext.Groups.RemoveFromGroupAsync(userId, groupName);

            // Remove user from active users
            _activeUsers.TryRemove(userId, out _);
        }

        // Retrieves all messages from a group (room)
        public Task<IEnumerable<string>> GetGroupMessagesAsync(string groupName) {
            if (_groupMessages.TryGetValue(groupName, out var messages)) {
                var messageList = messages.Select(m => $"{m.Timestamp}: {m.UserId}: {m.Message}");
                return Task.FromResult(messageList);
            }
            return Task.FromResult(Enumerable.Empty<string>());
        }

        // Retrieves private messages between two users
        public Task<IEnumerable<string>> GetPrivateMessagesAsync(string userId, string targetUserId) {
            if (_privateMessages.TryGetValue(userId, out var privateMessages)) {
                var messages = privateMessages.Where(m => m.ReceiverId == targetUserId || m.SenderId == targetUserId)
                                              .Select(m => $"{m.Timestamp}: {m.SenderId}: {m.Message}");

                return Task.FromResult(messages);
            }
            return Task.FromResult(Enumerable.Empty<string>());
        }

        // Notifies the group that a user is typing
        public async Task NotifyUserTypingAsync(string groupName, string userId) {
            await _hubContext.Clients.Group(groupName).SendAsync("UserTyping", userId);
        }

        // Retrieves the list of active users
        public Task<IEnumerable<string>> GetActiveUsersAsync() {
            return Task.FromResult(_activeUsers.Keys.AsEnumerable());
        }

        // Stores messages in memory for persistence (can be replaced with database logic)
        public Task AddMessageToHistoryAsync(string groupName, string userId, string message) {
            if (!_groupMessages.ContainsKey(groupName)) {
                _groupMessages[groupName] = new List<(string, string, DateTime)>();
            }

            _groupMessages[groupName].Add((userId, message, DateTime.UtcNow));
            return Task.CompletedTask;
        }
    }

}
