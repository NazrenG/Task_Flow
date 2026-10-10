using Microsoft.AspNetCore.SignalR;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Services.Notifications
{
    public class UserTaskRealtimeNotifier : IUserTaskRealtimeNotifier
    {
        private readonly IHubContext<ConnectionHub> _hubContext;

        public UserTaskRealtimeNotifier(IHubContext<ConnectionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyTaskCreatedAsync(string userId)
        {
            return SendAsync(userId,
                WorkHubEvents.TaskTotalCount,
                WorkHubEvents.OnHoldTaskCount,
                WorkHubEvents.UserTaskList);
        }

        public async Task NotifyTaskEditedAsync(string userId, string ownerId)
        {
            await SendAsync(userId,
                WorkHubEvents.UserTaskList,
                WorkHubEvents.OnHoldTaskCount,
                WorkHubEvents.RunningTaskCount,
                WorkHubEvents.CompletedTaskCount,
                // canban ucun signalr
                WorkHubEvents.CanbanTaskUpdated);

            // project, view detail, view profil ve dashboard sehifeleri
            await SendAsync(ownerId,
                WorkHubEvents.ProjectsTaskList,
                WorkHubEvents.ProjectDetailTaskList,
                WorkHubEvents.UserProfileTask,
                WorkHubEvents.DashboardReceiveProject);

            // project activity log signalr (detail ve project sehifeleri)
            await SendAsync(ownerId, WorkHubEvents.ProjectRecentActivityInDetail);
            await SendAsync(userId, WorkHubEvents.ProjectRecentActivityInDetail);
            await SendAsync(ownerId, WorkHubEvents.ProjectsRecentActivity);
            await SendAsync(userId, WorkHubEvents.ProjectsRecentActivity);
        }

        public Task NotifyCalendarTaskEditedAsync(string userId)
        {
            return SendAsync(userId, WorkHubEvents.UserTaskList);
        }

        public async Task NotifyTaskDeletedAsync(string userId, string? deletedTaskStatus)
        {
            var statusEvent = GetStatusCountEvent(deletedTaskStatus);
            if (statusEvent != null)
            {
                await SendAsync(userId, statusEvent);
            }

            await SendAsync(userId, WorkHubEvents.TaskTotalCount);
        }

        private static string? GetStatusCountEvent(string? status)
        {
            return status switch
            {
                "to do" => WorkHubEvents.OnHoldTaskCount,
                "in progress" => WorkHubEvents.RunningTaskCount,
                "done" => WorkHubEvents.CompletedTaskCount,
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
