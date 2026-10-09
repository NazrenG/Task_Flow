using Microsoft.AspNetCore.SignalR;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Services.Notifications
{
    public class WorkRealtimeNotifier : IWorkRealtimeNotifier
    {
        private readonly IHubContext<ConnectionHub> _hubContext;

        public WorkRealtimeNotifier(IHubContext<ConnectionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyTaskStatusUpdatedAsync(string userId)
        {
            await SendAsync(userId,
                WorkHubEvents.OnHoldTaskCount,
                WorkHubEvents.RunningTaskCount,
                WorkHubEvents.CompletedTaskCount);
        }

        public async Task NotifyTaskEditedByManagerAsync(string managerId, string memberId, string assigneeId)
        {
            // userin task listi ucun kanbandan gelen task
            await SendAsync(memberId, WorkHubEvents.UserTaskList);
            await SendAsync(assigneeId,
                WorkHubEvents.RunningTaskCount,
                WorkHubEvents.CompletedTaskCount,
                WorkHubEvents.OnHoldTaskCount);

            // canban ucun signalr
            await SendAsync(managerId, WorkHubEvents.CanbanTaskUpdated);
            await SendAsync(assigneeId, WorkHubEvents.CanbanTaskUpdated);
            await SendAsync(memberId, WorkHubEvents.DashboardCalendarNotificationCount);

            // project, view detail ve view profil sehifelerindeki task list
            await SendAsync(assigneeId,
                WorkHubEvents.ProjectsTaskList,
                WorkHubEvents.ProjectDetailTaskList,
                WorkHubEvents.UserProfileTask);

            // dashboard-da current project
            await SendAsync(memberId, WorkHubEvents.DashboardReceiveProject);

            await NotifyProjectActivityAsync(memberId, managerId);
        }

        public async Task NotifyTaskCreatedAsync(string managerId, string memberId, string assigneeId)
        {
            // userin task listi ucun kanbandan gelen task
            await SendAsync(memberId, WorkHubEvents.UserTaskList);
            await SendAsync(assigneeId,
                WorkHubEvents.TaskTotalCount,
                WorkHubEvents.OnHoldTaskCount);

            // canban ucun signalr
            await SendAsync(managerId, WorkHubEvents.CanbanTaskUpdated);
            await SendAsync(assigneeId, WorkHubEvents.CanbanTaskUpdated);
            await SendAsync(memberId, WorkHubEvents.DashboardCalendarNotificationCount);

            // project, view detail ve view profil sehifelerindeki task list
            await SendAsync(assigneeId,
                WorkHubEvents.ProjectsTaskList,
                WorkHubEvents.ProjectDetailTaskList,
                WorkHubEvents.UserProfileTask);

            // dashboard-da current project
            await SendAsync(memberId, WorkHubEvents.DashboardReceiveProject);

            // backlog
            await _hubContext.Clients.All.SendAsync(WorkHubEvents.UpdateBacklogTask);
        }

        public async Task NotifyTaskDeletedAsync(string managerId, string assigneeId)
        {
            await SendAsync(assigneeId,
                WorkHubEvents.UserTaskList,
                WorkHubEvents.TaskTotalCount,
                WorkHubEvents.RunningTaskCount,
                WorkHubEvents.CompletedTaskCount,
                WorkHubEvents.OnHoldTaskCount);

            // canban ucun signalr
            await SendAsync(managerId, WorkHubEvents.CanbanTaskUpdated);
            await SendAsync(assigneeId,
                WorkHubEvents.CanbanTaskUpdated,
                WorkHubEvents.ProjectsTaskList,
                WorkHubEvents.ProjectDetailTaskList,
                WorkHubEvents.UserProfileTask,
                WorkHubEvents.DashboardReceiveProject);

            await NotifyProjectActivityAsync(assigneeId, managerId);
        }

        public async Task NotifyRequestListsAsync(string receiverId)
        {
            await SendAsync(receiverId,
                WorkHubEvents.RequestList2,
                WorkHubEvents.RequestCount,
                WorkHubEvents.RequestList);
        }

        public async Task NotifyProjectActivityAsync(string memberId, string managerId)
        {
            // project activity log signalr detail sehifesi
            await SendAsync(memberId, WorkHubEvents.ProjectRecentActivityInDetail);
            await SendAsync(managerId, WorkHubEvents.ProjectRecentActivityInDetail);

            // project activity log signalr project sehifesi
            await SendAsync(memberId, WorkHubEvents.ProjectsRecentActivity);
            await SendAsync(managerId, WorkHubEvents.ProjectsRecentActivity);
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
