namespace Task_Flow.WebAPI.Services.Notifications
{
    /// <summary>
    /// Task-larla bağlı frontend-ə göndərilən SignalR event adları.
    /// </summary>
    public static class WorkHubEvents
    {
        // Task sayları
        public const string OnHoldTaskCount = "OnHoldTaskCount";
        public const string RunningTaskCount = "RunningTaskCount";
        public const string CompletedTaskCount = "CompletedTaskCount";
        public const string TaskTotalCount = "TaskTotalCount";

        // Task siyahıları
        public const string UserTaskList = "UserTaskList";
        public const string CanbanTaskUpdated = "CanbanTaskUpdated";
        public const string ProjectsTaskList = "ProjectsTaskList";
        public const string ProjectDetailTaskList = "ProjectDetailTaskList";
        public const string UserProfileTask = "UserProfileTask";
        public const string UpdateBacklogTask = "UpdateBacklogTask";

        // Dashboard
        public const string DashboardCalendarNotificationCount = "DashboardCalendarNotificationCount";
        public const string DashboardReceiveProject = "DashboardReceiveProject";

        // Project activity log
        public const string ProjectRecentActivityInDetail = "ProjectRecentActivityInDetail";
        public const string ProjectsRecentActivity = "ProjectsRecentActivity";

        // Request (notification) siyahısı
        public const string RequestList = "RequestList";
        public const string RequestList2 = "RequestList2";
        public const string RequestCount = "RequestCount";
    }
}
