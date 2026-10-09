using Task_Flow.Business.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Services.Notifications;

namespace Task_Flow.WebAPI.Services.NotificationCenter.RequestAcceptance
{
    // Dostluq sorğusu qəbul edildikdə istifadəçilər dost olur
    public class FriendRequestAcceptHandler : IRequestAcceptHandler
    {
        private readonly IFriendService _friendService;
        private readonly INotificationRealtimeNotifier _notifier;

        public FriendRequestAcceptHandler(IFriendService friendService, INotificationRealtimeNotifier notifier)
        {
            _friendService = friendService;
            _notifier = notifier;
        }

        public string NotificationType => RequestNotificationTypes.FriendRequest;

        public async Task HandleAsync(RequestNotification request, string userId)
        {
            await _friendService.Add(new Friend
            {
                UserId = request.SenderId,
                UserFriendId = userId,
                IsFriend = true
            });

            await _notifier.NotifyFriendListAsync(request.SenderId!);
        }
    }
}
