using Microsoft.AspNetCore.Mvc;
using PhotocopyRevaluationAppMVC.Data;
using PhotocopyRevaluationAppMVC.Services;

namespace PhotocopyRevaluationAppMVC.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly PhotocopyRevaluationAppContext _context;

        private readonly INotificationService _notificationService;
        private readonly IUserSessionService _sessionService;

        public NotificationsController(INotificationService notificationService, IUserSessionService sessionService, PhotocopyRevaluationAppContext context)
        {
            _notificationService = notificationService;
            _sessionService = sessionService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> SesstionTimedOut()
        {
            return View();
        }

        // GET: Notifications
        public async Task<IActionResult> Index(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("ApplicationUser ID cannot be null or empty.");
            }

            //userId = User?.Identity?.Name; // Get current user id
            var notifications = await _notificationService.GetSignOutNotificationsForUserAsync(userId);

            return View(Convert.ToInt32(notifications)); // Make sure you have a corresponding View to display these notifications
        }

        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            await MarkAsRead(Convert.ToInt32(notificationId));
            return Ok();
        }

        public async Task<bool> InvalidateUserSession(string SessionId)
        {
            return await _sessionService.DeleteUserSessionDataAsync(SessionId);
        }
    }
}
