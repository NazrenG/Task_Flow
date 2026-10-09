using Microsoft.AspNetCore.SignalR;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Services.Notifications
{
    public class ProjectRealtimeNotifier : IProjectRealtimeNotifier
    {
        private readonly IHubContext<ConnectionHub> _hubContext;

        public ProjectRealtimeNotifier(IHubContext<ConnectionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyProjectCreatedAsync(string userId, string? status)
        {
            await SendAsync(userId,
                ProjectHubEvents.ReceiveProjectUpdate,
                ProjectHubEvents.RecieveInProgressUpdate,
                ProjectHubEvents.UpdateTotalProjects);

            var statusEvent = GetStatusEvent(status);
            if (statusEvent != null)
            {
                await SendAsync(userId, statusEvent);
            }
        }

        public async Task NotifyProjectUpdatedAsync(string userId)
        {
            await SendAsync(userId,
                ProjectHubEvents.ReceiveProjectUpdate,
                ProjectHubEvents.RecieveInProgressUpdate);

            await _hubContext.Clients.All.SendAsync(ProjectHubEvents.ReceiveProjectUpdateDashboard);
        }

        public async Task NotifyProjectDeletedAsync(string userId)
        {
            await SendAsync(userId,
                ProjectHubEvents.RecieveInProgressUpdate,
                ProjectHubEvents.RequestList);
        }

        public Task NotifyProjectMembersChangedAsync(string userId)
        {
            return SendAsync(userId, ProjectHubEvents.ReceiveProjectUpdate);
        }

        private static string? GetStatusEvent(string? status)
        {
            return status switch
            {
                "On Going" => ProjectHubEvents.UpdateOnGoingProjects,
                "Pending" => ProjectHubEvents.UpdatePendingProjects,
                "Completed" => ProjectHubEvents.UpdateCompletedProjects,
                _ => null
            };
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
