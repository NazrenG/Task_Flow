using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.NotificationCenter
{
    /// <summary>
    /// İstifadəçinin bildiriş ayarları.
    /// </summary>
    public interface INotificationSettingAppService
    {
        Task<ServiceResult<object>> EnsureSettingAsync(string userId);
        Task<ServiceResult<object>> UpdateSettingAsync(string userId, NotificationSettingDto dto);
    }
}
