using Task_Flow.Business.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.NotificationCenter
{
    public class CalendarNotificationAppService : ICalendarNotificationAppService
    {
        private const int LatestCount = 2;

        private readonly INotificationService _notificationService;
        private readonly INotificationRealtimeNotifier _notifier;

        public CalendarNotificationAppService(
            INotificationService notificationService,
            INotificationRealtimeNotifier notifier)
        {
            _notificationService = notificationService;
            _notifier = notifier;
        }

        public async Task<ServiceResult<List<object>>> GetCalendarNotificationsAsync(string userId)
        {
            var notifications = await GetUserCalendarNotificationsAsync(userId);
            return ServiceResult<List<object>>.Success(notifications.Select(n => n.ToCalendarItem()).ToList());
        }

        public async Task<ServiceResult<List<object>>> GetLatestCalendarNotificationsAsync(string userId)
        {
            var notifications = await GetUserCalendarNotificationsAsync(userId);
            var latest = notifications
                .OrderByDescending(n => n.Id)
                .Take(LatestCount)
                .Select(n => n.ToCalendarSummary())
                .ToList();

            return ServiceResult<List<object>>.Success(latest);
        }

        public async Task<ServiceResult<int>> GetCalendarNotificationCountAsync(string? userId)
        {
            var notifications = await GetUserCalendarNotificationsAsync(userId);
            return ServiceResult<int>.Success(notifications.Count);
        }

        public async Task<ServiceResult<object>> DeleteCalendarNotificationAsync(int id, string userId)
        {
            var notification = await _notificationService.GetNotificationById(id);
            if (notification == null)
            {
                return ServiceResult<object>.BadRequest(new { message = "not found message" });
            }

            await _notificationService.Delete(notification);
            await _notifier.NotifyCalendarNotificationsAsync(userId);
            // userin loglari
            await _notifier.NotifyRecentActivityAsync(userId);

            return ServiceResult<object>.Success(new { message = "delete message succesfuly" });
        }

        public async Task<ServiceResult<Notification>> AddAsync(NotificationDto value)
        {
            var notification = new Notification
            {
                Text = value.Text,
                UserId = value.UserId
            };
            await _notificationService.Add(notification);

            return ServiceResult<Notification>.Success(notification);
        }

        public async Task<ServiceResult<Empty>> DeleteAsync(int id)
        {
            var notification = await _notificationService.GetNotificationById(id);
            if (notification == null)
            {
                return ServiceResult<Empty>.NotFound();
            }

            await _notificationService.Delete(notification);
            return ServiceResult<Empty>.Success(Empty.Value);
        }

        private async Task<List<Notification>> GetUserCalendarNotificationsAsync(string? userId)
        {
            var notifications = await _notificationService.GetNotifications();
            return notifications.Where(n => n.UserId == userId && n.IsCalendarMessage).ToList();
        }
    }
}
