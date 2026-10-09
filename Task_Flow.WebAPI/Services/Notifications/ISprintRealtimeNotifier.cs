namespace Task_Flow.WebAPI.Services.Notifications
{
    /// <summary>
    /// Sprint əməliyyatlarından sonra istifadəçinin ekranını SignalR ilə yeniləyir.
    /// </summary>
    public interface ISprintRealtimeNotifier
    {
        Task NotifySprintsChangedAsync(string userId);
        Task NotifyTaskSprintChangedAsync(string userId);
    }
}
