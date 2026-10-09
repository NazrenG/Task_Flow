using Task_Flow.Business.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.NotificationCenter
{
    public class NotificationSettingAppService : INotificationSettingAppService
    {
        private readonly INotificationSettingService _notificationSettingService;
        private readonly INotificationRealtimeNotifier _notifier;

        public NotificationSettingAppService(
            INotificationSettingService notificationSettingService,
            INotificationRealtimeNotifier notifier)
        {
            _notificationSettingService = notificationSettingService;
            _notifier = notifier;
        }

        // Ayar yoxdursa, default ayar yaradılır
        public async Task<ServiceResult<object>> EnsureSettingAsync(string userId)
        {
            await _notificationSettingService.GetOrCreateNotificationSetting(userId);
            return ServiceResult<object>.Success(new { success = true, message = "notification setting" });
        }

        public async Task<ServiceResult<object>> UpdateSettingAsync(string userId, NotificationSettingDto dto)
        {
            var setting = await _notificationSettingService.GetNotificationSetting(userId);

            if (setting == null)
            {
                var newSetting = new NotificationSetting { UserId = userId };
                dto.ApplyTo(newSetting);
                await _notificationSettingService.Add(newSetting);

                return ServiceResult<object>.Success(new { message = "new notification service." });
            }

            dto.ApplyTo(setting);
            await _notificationSettingService.Update(setting);
            await _notifier.NotifyRecentActivityAsync(userId);

            return ServiceResult<object>.Success(new { success = true, message = "Update successful" });
        }
    }
}
