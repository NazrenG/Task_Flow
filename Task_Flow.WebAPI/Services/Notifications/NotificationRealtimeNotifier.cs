using Microsoft.AspNetCore.SignalR;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Services.Notifications
{
    public class NotificationRealtimeNotifier : INotificationRealtimeNotifier
    {
        private readonly IHubContext<ConnectionHub> _hubContext;

        public NotificationRealtimeNotifier(IHubContext<ConnectionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyRequestListsAsync(string userId)
        {
            return SendAsync(userId,
                NotificationHubEvents.RequestList2,
                NotificationHubEvents.RequestCount,
                NotificationHubEvents.RequestList);
        }

        public Task NotifyCalendarNotificationsAsync(string userId)
        {
            return SendAsync(userId,
                NotificationHubEvents.ReminderRequestList,
                NotificationHubEvents.CalendarNotificationCount,
                NotificationHubEvents.CalendarNotificationList2);
        }

        public Task NotifyRecentActivityAsync(string userId)
        {
            return SendAsync(userId, NotificationHubEvents.RecentActivityUpdate);
        }

        public Task NotifyUserActivityAsync(string userId)
        {
            return SendAsync(userId, NotificationHubEvents.UpdateUserActivity);
        }

        public Task NotifyFollowRequestSentAsync(string senderId, string receiverId)
        {
            return _hubContext.Clients.User(senderId).SendAsync(NotificationHubEvents.InvokeSendFollow, receiverId);
        }

        public Task NotifyFriendListAsync(string userId)
        {
            return SendAsync(userId, NotificationHubEvents.UpdateMessageFriendList);
        }

        private async Task SendAsync(string userId, params string[] events)
        {
            foreach (var hubEvent in events)
            {
                await _hubContext.Clients.User(userId).SendAsync(hubEvent);
            }
        }
    }
}
