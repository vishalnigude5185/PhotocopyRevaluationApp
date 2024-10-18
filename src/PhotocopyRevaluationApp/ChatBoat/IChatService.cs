namespace PhotocopyRevaluationApp.ChatBoat {
    public interface IChatService {
        Task SaveMessageAsync(string groupName, string userId, string message, DateTime timestamp);
        Task MarkMessageAsReadAsync(int messageId, string userId);
        Task<bool> SendMessageToGroupAsync(string groupName, string userId, string message);
        Task<bool> SendMessageToUserAsync(string receiverUserId, string senderUserId, string message);
        Task JoinGroupAsync(string groupName, string userId);
        Task LeaveGroupAsync(string groupName, string userId);
        Task<IEnumerable<string>> GetGroupMessagesAsync(string groupName);
        Task<IEnumerable<string>> GetPrivateMessagesAsync(string userId, string targetUserId);
        Task NotifyUserTypingAsync(string groupName, string userId);
        Task<IEnumerable<string>> GetActiveUsersAsync();
        Task AddMessageToHistoryAsync(string groupName, string userId, string message);
    }

}
