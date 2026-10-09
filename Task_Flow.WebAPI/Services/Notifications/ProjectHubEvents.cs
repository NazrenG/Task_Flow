namespace Task_Flow.WebAPI.Services.Notifications
{
    /// <summary>
    /// Layihələrlə bağlı frontend-ə göndərilən SignalR event adları.
    /// </summary>
    public static class ProjectHubEvents
    {
        public const string ReceiveProjectUpdate = "ReceiveProjectUpdate";
        public const string RecieveInProgressUpdate = "RecieveInProgressUpdate";
        public const string UpdateTotalProjects = "UpdateTotalProjects";
        public const string ReceiveProjectUpdateDashboard = "ReceiveProjectUpdateDashboard";

        public const string UpdateOnGoingProjects = "UpdateOnGoingProjects";
        public const string UpdatePendingProjects = "UpdatePendingProjects";
        public const string UpdateCompletedProjects = "UpdateCompletedProjects";

        public const string RequestList = "RequestList";
        public const string RecieveRecentActivityUpdate = "RecieveRecentActivityUpdate";
    }
}
