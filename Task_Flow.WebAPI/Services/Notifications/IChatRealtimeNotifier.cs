namespace Task_Flow.WebAPI.Services.Notifications
{
    /// <summary>
    /// Şəxsi çat əməliyyatlarından sonra istifadəçilərin ekranlarını SignalR ilə yeniləyir.
    /// </summary>
    public interface IChatRealtimeNotifier
    {
        Task NotifyMessageSentAsync(string senderId, string friendEmail);
    }
}
