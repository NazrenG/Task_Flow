using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Services.Notifications
{
    /// <summary>
    /// Qrup çatı əməliyyatlarından sonra istifadəçilərin ekranlarını SignalR ilə yeniləyir.
    /// </summary>
    public interface IGroupChatRealtimeNotifier
    {
        Task SendGroupMessageAsync(int groupId, GroupChatMessageDto message);
        Task NotifyGroupMembersChangedAsync(string userId);
        Task NotifyGroupListChangedAsync(string userId);
    }
}
