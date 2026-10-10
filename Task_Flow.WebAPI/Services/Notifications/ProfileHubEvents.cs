namespace Task_Flow.WebAPI.Services.Notifications
{
    /// <summary>
    /// Profil ilə bağlı SignalR event adları.
    /// </summary>
    public static class ProfileHubEvents
    {
        public const string ProfileUpdated = "ProfileUpdated";
        public const string UpdateUserActivity = "UpdateUserActivity";
        public const string ReceiveConnectInfo = "ReceiveConnectInfo";
    }
}
