namespace Task_Flow.WebAPI.Services.Notifications
{
    /// <summary>
    /// İstifadəçinin şəxsi task əməliyyatlarından sonra ekranları SignalR ilə yeniləyir.
    /// </summary>
    public interface IUserTaskRealtimeNotifier
    {
        Task NotifyTaskCreatedAsync(string userId);
        Task NotifyTaskEditedAsync(string userId, string ownerId);
        Task NotifyCalendarTaskEditedAsync(string userId);
        Task NotifyTaskDeletedAsync(string userId, string? deletedTaskStatus);
    }
}
