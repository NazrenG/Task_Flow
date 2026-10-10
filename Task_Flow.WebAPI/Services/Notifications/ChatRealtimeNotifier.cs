using Microsoft.AspNetCore.SignalR;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Services.Notifications
{
    public class ChatRealtimeNotifier : IChatRealtimeNotifier
    {
        private const string ReceiveMessages = "ReceiveMessages2";

        private readonly IHubContext<ConnectionHub> _hubContext;

        public ChatRealtimeNotifier(IHubContext<ConnectionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyMessageSentAsync(string senderId, string friendEmail)
        {
            return _hubContext.Clients.User(senderId).SendAsync(ReceiveMessages, friendEmail);
        }
    }
}
