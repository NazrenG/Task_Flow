using Task_Flow.Business.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.NotificationCenter
{
    public class RecentActivityAppService : IRecentActivityAppService
    {
        private const string NotificationActivityType = "Notification";

        private readonly IRecentActivityService _recentActivityService;
        private readonly INotificationRealtimeNotifier _notifier;

        public RecentActivityAppService(
            IRecentActivityService recentActivityService,
            INotificationRealtimeNotifier notifier)
        {
            _recentActivityService = recentActivityService;
            _notifier = notifier;
        }

        public async Task<ServiceResult<List<RecentActivityDto>>> GetRecentActivitiesAsync(string userId)
        {
            var activities = await _recentActivityService.GetRecentActivities(userId);
            return ServiceResult<List<RecentActivityDto>>.Success(
                activities.Select(a => a.ToRecentActivityDto()).ToList());
        }

        public async Task<ServiceResult<object>> AddAsync(string userId, RecentActivityDto dto)
        {
            await _recentActivityService.Add(new RecentActivity
            {
                UserId = userId,
                Text = dto.Text,
                Type = dto.Type
            });

            return ServiceResult<object>.Success(new { message = "Activity added successfully" });
        }

        public async Task LogNotificationActivityAsync(string userId, string text)
        {
            await _recentActivityService.Add(new RecentActivity
            {
                UserId = userId,
                Text = text,
                Type = NotificationActivityType
            });
            await _notifier.NotifyRecentActivityAsync(userId);
        }
    }
}
