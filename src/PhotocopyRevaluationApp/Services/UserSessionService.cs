using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using PhotocopyRevaluationApp.Models;

namespace PhotocopyRevaluationApp.Services {
    public class UserSessionService : IUserSessionService {
        private readonly IMemoryCache _memoryCache;
        private readonly ConcurrentDictionary<int, bool> _memoryCacheKeys = new();

        private readonly ConcurrentDictionary<string, SessionData> _userSessionsData = new ConcurrentDictionary<string, SessionData>();

        private readonly HashSet<object> _keys = new HashSet<object>();

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSessionService(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache) {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }

        //===================================== cureent required =============================================
        // Store session information in the cache
        public void CreateUserSessionDataAsync(string SessionId, SessionData SessionData) {
            if (SessionData != null) {
                // Store the session in the cache, with the expiration time
                _userSessionsData.TryAdd(SessionId, SessionData);
            }
        }
        // Store user session information in MemoryCache`
        public void AddToMemoryCache(int key, DateTimeOffset value) {
            _memoryCache.Set(key, value);
            _memoryCacheKeys.TryAdd(key, true);  // Track the cache key
        }

        public async Task<bool> DeleteUserSessionDataAsync(string SessionId) {
            if (_userSessionsData.TryRemove(SessionId, out _)) {
                // Session was found and removed
                return await Task.FromResult(true);
            }

            // Session not found
            return await Task.FromResult(false);
        }
        public void RemoveFromMemoryCache(string key) {
            _memoryCache.Remove(key);
            _memoryCacheKeys.TryRemove(Convert.ToInt32(key), out _);
        }
        public SessionData? GetSessionDataBySessionId<SessionData>(string sessionId) {
            _memoryCache.TryGetValue(sessionId, out SessionData? data);
            return data;
        }

        // Retrieve a list of all user IDs with their expiration times
        public ConcurrentDictionary<string, SessionData> GetAllUsersSessionDataAsync() => _userSessionsData;
        //===================================== cureent required end =============================================
        public string? GetCurrentUserId() {
            // Access HttpContext to get the currently authenticated user's name or ID
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            //// This could fetch the userId from the current HttpContext, assuming authenticated users.
            //return HttpContext.Current?.ApplicationUser?.Identity?.Name; // Adjust according to your auth mechanism
        }

        //added from chat
        //To get the one user userId
        public string GetUserId() {
            // Access the authenticated user's ClaimsPrincipal
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId)) {
                return userId;
            }

            return null; // User is not authenticated or no user ID found
        }
        //To get the one user 
        public async Task<DateTimeOffset?> GetSessionExpirationAsync() {
            var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();

            if (authenticateResult?.Properties?.ExpiresUtc != null) {
                return authenticateResult.Properties.ExpiresUtc;
            }

            return null; // No expiration found or user is not authenticated
        }

        //public string GetUserId()
        //{
        //    var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    return userId;
        //}

        public DateTimeOffset? GetSessionExpiration() {
            var authenticationProperties = _httpContextAccessor.HttpContext.AuthenticateAsync().Result?.Properties;
            return authenticationProperties?.ExpiresUtc;
        }

        public bool IsUserLoggedIn() {
            return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
