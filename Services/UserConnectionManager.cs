using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PhotocopyRevaluationAppMVC.Services
{
    public class UserConnectionManager : IUserConnectionManager
    {
        // Use a thread-safe collection for user-connection mappings
        private readonly ConcurrentDictionary<string, List<string>> _userConnections = new();

        public UserConnectionManager()
        {

        }
        public Task AddConnectionAsync(string userId, string connectionId)
        {
            //_userConnections[userId] = connectionId;
            _userConnections.AddOrUpdate(userId,
         new List<string> { connectionId }, // If userId does not exist, create a new list with the connectionId.
         (userId, existingList) =>
         {
             // If userId exists, check if the connectionId is already in the list
             if (!existingList.Contains(connectionId))
             {
                 existingList.Add(connectionId); // Add the connectionId if it's not already present.
             }
             return existingList; // Return the updated list.
         });
            return Task.CompletedTask;
        }

        public Task RemoveConnectionAsync(string userId)
        {
            if (_userConnections.ContainsKey(userId))
            {
                _userConnections.TryRemove(userId, out _);
            }

            //var item = _userConnections.FirstOrDefault(u => u.Value == connectionId);
            //if (!string.IsNullOrEmpty(item.Key))
            //{
            //    _userConnections.TryRemove(item.Key, out _);
            //}

            return Task.CompletedTask;
        }

        //to get the single connection for user
        public Task<List<string>?> GetConnectionIdsAsync(string userId)
        {
            _userConnections.TryGetValue(userId, out List<string>? connectionId);
            return Task.FromResult(connectionId);

            //_userConnections.TryGetValue(userId, out var connectionId);
            //return connectionId;
        }
    }
}
