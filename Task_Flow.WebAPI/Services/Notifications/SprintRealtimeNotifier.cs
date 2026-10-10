using Microsoft.AspNetCore.SignalR;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Services.Notifications
{
    public class SprintRealtimeNotifier : ISprintRealtimeNotifier
    {
        private const string UpdateSprints = "UpdateSprints";
        private const string AddProjectToSprint = "AddProjectToSprint";

        private readonly IHubContext<ConnectionHub> _hubContext;

        public SprintRealtimeNotifier(IHubContext<ConnectionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // backlog
        public Task NotifySprintsChangedAsync(string userId)
        {
            return _hubContext.Clients.User(userId).SendAsync(UpdateSprints);
        }

        public Task NotifyTaskSprintChangedAsync(string userId)
        {
            return _hubContext.Clients.User(userId).SendAsync(AddProjectToSprint);
        }
    }
}
