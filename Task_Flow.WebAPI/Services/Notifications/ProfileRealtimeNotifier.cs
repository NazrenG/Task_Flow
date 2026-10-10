using Microsoft.AspNetCore.SignalR;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Services.Notifications
{
    public class ProfileRealtimeNotifier : IProfileRealtimeNotifier
    {
        private readonly IHubContext<ConnectionHub> _hubContext;

        public ProfileRealtimeNotifier(IHubContext<ConnectionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyProfileUpdatedAsync(string userId)
        {
            return _hubContext.Clients.User(userId).SendAsync(ProfileHubEvents.ProfileUpdated);
        }

        // İstifadəçinin online/offline statusu hamıya göstərilir
        public Task NotifyUserActivityChangedForAllAsync()
        {
            return _hubContext.Clients.All.SendAsync(ProfileHubEvents.UpdateUserActivity);
        }

        // Sistemə daxil olan istifadəçi haqqında hamıya məlumat göndərilir
        public Task NotifyUserConnectedAsync(string username)
        {
            return _hubContext.Clients.All.SendAsync(ProfileHubEvents.ReceiveConnectInfo, $"{username} has connected");
        }
    }
}
