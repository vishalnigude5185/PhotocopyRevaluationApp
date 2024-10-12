using PhotocopyRevaluationAppMVC.Models;
using System.Collections.Concurrent;

namespace PhotocopyRevaluationAppMVC.Services
{
    public interface IUserSessionService
    {
        SessionData GetSessionDataBySessionId<SessionData>(string SessionId);
        void CreateUserSessionDataAsync(string SessionId,SessionData SessionData);
        ConcurrentDictionary<string, SessionData> GetAllUsersSessionDataAsync();
        Task<bool> DeleteUserSessionDataAsync(string sessionId);
        string GetCurrentUserId();
    }
}
