using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.NotificationCenter
{
    /// <summary>
    /// İstifadəçinin təqvim (Notification) bildirişləri.
    /// </summary>
    public interface ICalendarNotificationAppService
    {
        Task<ServiceResult<List<object>>> GetCalendarNotificationsAsync(string userId);
        Task<ServiceResult<List<object>>> GetLatestCalendarNotificationsAsync(string userId);
        Task<ServiceResult<int>> GetCalendarNotificationCountAsync(string? userId);
        Task<ServiceResult<object>> DeleteCalendarNotificationAsync(int id, string userId);
        Task<ServiceResult<Notification>> AddAsync(NotificationDto value);
        Task<ServiceResult<Empty>> DeleteAsync(int id);
    }
}
