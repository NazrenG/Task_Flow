namespace Task_Flow.WebAPI.Services.Notifications
{
    /// <summary>
    /// Profil əməliyyatlarından sonra istifadəçilərin ekranlarını SignalR ilə yeniləyir.
    /// </summary>
    public interface IProfileRealtimeNotifier
    {
        Task NotifyProfileUpdatedAsync(string userId);
        Task NotifyUserActivityChangedForAllAsync();
    }
}
