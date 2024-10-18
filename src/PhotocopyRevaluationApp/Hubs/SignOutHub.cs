using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using PhotocopyRevaluationApp.Services;

namespace PhotocopyRevaluationApp.Hubs {
    public class SignOutHub : Hub {
        private readonly IUserConnectionManager _connectionManager;
        public SignOutHub(IUserConnectionManager connectionManager) {
            _connectionManager = connectionManager;
        }

        //1) Aproach
        public override async Task OnConnectedAsync() {
            ////Check if the user has a unique ID stored in a cookie
            //var userId = Context.UserIdentifier; // Optional: Get the user's identifier
            ////Retrieve the authenticated user ID from claims
            string userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var context = Context.GetHttpContext();
            userId = context.Request.Cookies["userId"];
            //// Generate a temporary ID (e.g., a GUID) and store it in a cookie
            //userId = Guid.NewGuid().ToString();
            //context.Response.Cookies.Append("UserTempId", userId);
            var connectionId = Context.ConnectionId; // Accessing ConnectionId from SignalR's Hub Context

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(connectionId)) {
                // Log the connection (for debugging or tracking purposes)
                Console.WriteLine($"User {userId} connected with connection ID: {connectionId}");
                // Optionally, store the connection ID for later use
                await _connectionManager.AddConnectionAsync(userId, connectionId);
                Console.WriteLine($"{_connectionManager.GetConnectionIdsAsync(userId)}");
            }
            else {
                throw new ArgumentNullException(nameof(connectionId), "Connection ID or UserId cannot be null or empty.");
            }
            await base.OnConnectedAsync();

            //_notificationService.CheckSessionExpirations();
            //_notificationService.CheckSessionExpirationsContinue();
        }
        //2) Aproach
        // switching from a temporary ID to a user ID after login:
        //public async Task OnUserAuthenticated(string actualUserId)
        //{
        //    var connectionId = Context.ConnectionId;

        //    // Remove the temporary ID and re-add the actual user ID
        //    await _connectionManager.RemoveConnectionAsync(connectionId);
        //    await _connectionManager.AddConnectionAsync(actualUserId, connectionId);
        //}

        //3) Aproach
        //Adding connection with ipAddress
        //public override async Task OnConnectedAsync()
        //{
        //    var connectionId = Context.ConnectionId;
        //    var ipAddress = Context.GetHttpContext()?.Connection.RemoteIpAddress.ToString();

        //    if (!string.IsNullOrEmpty(ipAddress))
        //    {
        //        await _connectionManager.AddConnectionAsync(ipAddress, connectionId);
        //    }

        //    await base.OnConnectedAsync();
        //}
        public override async Task OnDisconnectedAsync(Exception exception) {
            ////Check if the user has a unique ID stored in a cookie
            //var userId = Context.UserIdentifier; // Optional: Get the user's identifier
            ////Retrieve the authenticated user ID from claims
            string userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";

            var context = Context.GetHttpContext();
            // Check for null value and provide a default value if the cookie is not found
            userId = context.Request.Cookies["userId"] ?? "DefaultUserId";

            //// Generate a temporary ID (e.g., a GUID) and store it in a cookie
            //userId = Guid.NewGuid().ToString();
            var connectionId = Context.ConnectionId; // Get the connection ID for the user who disconnected

            // Log the disconnection
            Console.WriteLine($"User {userId} disconnected with connection ID: {connectionId}");

            // Clean up or remove the connection from your connection manager
            await _connectionManager.RemoveConnectionAsync(userId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
