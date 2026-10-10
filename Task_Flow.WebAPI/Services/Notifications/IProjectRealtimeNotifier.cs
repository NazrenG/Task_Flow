namespace Task_Flow.WebAPI.Services.Notifications
{
    /// <summary>
    /// Layihə əməliyyatlarından sonra istifadəçilərin ekranlarını SignalR ilə yeniləyir.
    /// </summary>
    public interface IProjectRealtimeNotifier
    {
        Task NotifyProjectCreatedAsync(string userId, string? status);
        Task NotifyProjectUpdatedAsync(string userId);
        Task NotifyProjectDeletedAsync(string userId);
        Task NotifyProjectMembersChangedAsync(string userId);
        Task NotifyProjectActivityAddedAsync(string projectOwnerId);
    }
}
