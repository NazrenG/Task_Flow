namespace Task_Flow.WebAPI.Services.Notifications
{
    /// <summary>
    /// Bildiriş və sorğu əməliyyatlarından sonra istifadəçilərin ekranlarını SignalR ilə yeniləyir.
    /// </summary>
    public interface INotificationRealtimeNotifier
    {
        Task NotifyRequestListsAsync(string userId);
        Task NotifyCalendarNotificationsAsync(string userId);
        Task NotifyRecentActivityAsync(string userId);
        Task NotifyUserActivityAsync(string userId);
        Task NotifyFollowRequestSentAsync(string senderId, string receiverId);
        Task NotifyFriendListAsync(string userId);
    }
}
