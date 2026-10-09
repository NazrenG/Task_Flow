using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.NotificationCenter
{
    /// <summary>
    /// İstifadəçinin son fəaliyyətləri (activity log).
    /// </summary>
    public interface IRecentActivityAppService
    {
        Task<ServiceResult<List<RecentActivityDto>>> GetRecentActivitiesAsync(string userId);
        Task<ServiceResult<object>> AddAsync(string userId, RecentActivityDto dto);

        // Digər servislərin istifadəsi üçün: log yazır və istifadəçinin ekranını yeniləyir
        Task LogActivityAsync(string userId, string text, string type);
        Task LogNotificationActivityAsync(string userId, string text);
    }
}
