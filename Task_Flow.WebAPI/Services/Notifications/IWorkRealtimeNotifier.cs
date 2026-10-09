namespace Task_Flow.WebAPI.Services.Notifications
{
    /// <summary>
    /// Task əməliyyatlarından sonra istifadəçilərin ekranlarını SignalR ilə yeniləyir.
    /// </summary>
    public interface IWorkRealtimeNotifier
    {
        Task NotifyTaskStatusUpdatedAsync(string userId);
        Task NotifyTaskEditedByManagerAsync(string managerId, string memberId, string assigneeId);
        Task NotifyTaskCreatedAsync(string managerId, string memberId, string assigneeId);
        Task NotifyTaskDeletedAsync(string managerId, string assigneeId);
        Task NotifyRequestListsAsync(string receiverId);
        Task NotifyProjectActivityAsync(string memberId, string managerId);
    }
}
