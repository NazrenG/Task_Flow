using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.NotificationCenter;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ICalendarNotificationAppService _calendarNotificationService;
        private readonly INotificationSettingAppService _notificationSettingService;
        private readonly IRecentActivityAppService _recentActivityService;
        private readonly IRequestNotificationAppService _requestNotificationService;

        public NotificationController(
            ICalendarNotificationAppService calendarNotificationService,
            INotificationSettingAppService notificationSettingService,
            IRecentActivityAppService recentActivityService,
            IRequestNotificationAppService requestNotificationService)
        {
            _calendarNotificationService = calendarNotificationService;
            _notificationSettingService = notificationSettingService;
            _recentActivityService = recentActivityService;
            _requestNotificationService = requestNotificationService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        private IActionResult UserNotAuthenticated() => BadRequest(new { message = "User not authenticated." });

        private IActionResult UserNotFound(string message = "user not found") => Unauthorized(new { message });

        // GET: api/<NotificationController>
        // userin bildirimleri
        [Authorize]
        [HttpGet("Notifications")]
        public async Task<IActionResult> Get()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _requestNotificationService.GetPendingSummariesAsync(userId));
        }

        [Authorize]
        [HttpGet("TwoNotification")]
        public async Task<IActionResult> TakeTwoMessage()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _requestNotificationService.GetLatestPendingSummariesAsync(userId));
        }

        [Authorize]
        [HttpGet("CalendarNotifications")]
        public async Task<IActionResult> GetCalendarNotifications()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _calendarNotificationService.GetCalendarNotificationsAsync(userId));
        }

        [Authorize]
        [HttpDelete("DeletedCalendarMessage/{id}")]
        public async Task<IActionResult> DeletedCalendarMessage(int id)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _calendarNotificationService.DeleteCalendarNotificationAsync(id, userId));
        }

        [Authorize]
        [HttpGet("TwoCalendarNotification")]
        public async Task<IActionResult> TakeTwoCalendarNotification()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _calendarNotificationService.GetLatestCalendarNotificationsAsync(userId));
        }

        // userin bildirim sayi
        [Authorize]
        [HttpGet("UserNotificationCount")]
        public async Task<IActionResult> GetCount()
        {
            return this.ToActionResult(await _requestNotificationService.GetPendingCountAsync(CurrentUserId));
        }

        // userin calendar ucun olan bildirim sayi
        [Authorize]
        [HttpGet("CalendarNotificationCount")]
        public async Task<IActionResult> GetCalendarNotificationCount()
        {
            return this.ToActionResult(await _calendarNotificationService.GetCalendarNotificationCountAsync(CurrentUserId));
        }

        // POST api/<NotificationController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NotificationDto value)
        {
            return this.ToActionResult(await _calendarNotificationService.AddAsync(value));
        }

        // DELETE api/<NotificationController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return this.ToActionResult(await _calendarNotificationService.DeleteAsync(id));
        }

        // notification setting
        [Authorize]
        [HttpGet("NotificationSetting")]
        public async Task<IActionResult> GetNotificationSetting()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotFound();

            return this.ToActionResult(await _notificationSettingService.EnsureSettingAsync(userId));
        }

        [Authorize]
        [HttpPost("UpdatedNotificationSetting")]
        public async Task<IActionResult> UpdateNotificationSetting(NotificationSettingDto dto)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotFound("User not found");

            return this.ToActionResult(await _notificationSettingService.UpdateSettingAsync(userId, dto));
        }

        //////// Recent Activity ////////
        [Authorize]
        [HttpGet("RecentActivity")]
        public async Task<IActionResult> GetRecentActivity()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotFound();

            return this.ToActionResult(await _recentActivityService.GetRecentActivitiesAsync(userId));
        }

        [Authorize]
        [HttpPost("NewRecentActivity")]
        public async Task<IActionResult> AddRecentActivity(RecentActivityDto dto)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotFound();

            return this.ToActionResult(await _recentActivityService.AddAsync(userId, dto));
        }

        // request notification
        [Authorize]
        [HttpGet("RequestNotification")]
        public async Task<IActionResult> GetRequestNotification()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotFound();

            return this.ToActionResult(await _requestNotificationService.GetPendingRequestsAsync(userId));
        }

        [Authorize]
        [HttpPost("NewRequestNotification")]
        public async Task<IActionResult> AddRequestNotification(RequestNotificationDto dto)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotFound();

            return this.ToActionResult(await _requestNotificationService.SendRequestAsync(userId, dto));
        }

        [Authorize]
        [HttpPost("NewCompanyRequestNotification")]
        public async Task<IActionResult> NewCompanyRequestNotification(CompanyRequestDto dto)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotFound();

            return this.ToActionResult(await _requestNotificationService.SendCompanyWorkerRequestsAsync(userId, dto));
        }

        [Authorize]
        [HttpDelete("DeleteRequestNotification/{requestId}")]
        public async Task<IActionResult> DeleteRequestNotification(int requestId)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotFound();

            return this.ToActionResult(await _requestNotificationService.DeleteRequestAsync(requestId, userId));
        }

        [Authorize]
        [HttpPut("AcceptRequestNotification/{requestId}")]
        public async Task<IActionResult> PutAcceptRequestNotification(int requestId)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotFound();

            return this.ToActionResult(await _requestNotificationService.AcceptRequestAsync(requestId, userId));
        }
    }
}
