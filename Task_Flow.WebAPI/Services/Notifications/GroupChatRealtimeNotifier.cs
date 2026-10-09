using Microsoft.AspNetCore.SignalR;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Services.Notifications
{
    public class GroupChatRealtimeNotifier : IGroupChatRealtimeNotifier
    {
        private const string ReceiveGroupMessage = "ReceiveGroupMessage";
        private const string UpdateGroupChatModal = "UpdateGroupChatModal";
        private const string UpdateGroupChatList = "UpdateGroupChatList";

        private readonly IHubContext<ConnectionHub> _hubContext;

        public GroupChatRealtimeNotifier(IHubContext<ConnectionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task SendGroupMessageAsync(int groupId, GroupChatMessageDto message)
        {
            return _hubContext.Clients.Group(GetGroupName(groupId)).SendAsync(ReceiveGroupMessage, message);
        }

        public Task NotifyGroupMembersChangedAsync(string userId)
        {
            return _hubContext.Clients.User(userId).SendAsync(UpdateGroupChatModal);
        }

        public Task NotifyGroupListChangedAsync(string userId)
        {
            return _hubContext.Clients.User(userId).SendAsync(UpdateGroupChatList);
        }

        // ConnectionHub-da istifadə olunan SignalR qrup adı
        private static string GetGroupName(int groupId) => $"group-{groupId}";
    }
}
