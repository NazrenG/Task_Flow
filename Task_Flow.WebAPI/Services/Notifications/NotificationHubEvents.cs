namespace Task_Flow.WebAPI.Services.Notifications
{
    /// <summary>
    /// Bildirişlər, sorğular və istifadəçi fəaliyyəti ilə bağlı SignalR event adları.
    /// </summary>
    public static class NotificationHubEvents
    {
        // Request (sorğu) siyahısı
        public const string RequestList = "RequestList";
        public const string RequestList2 = "RequestList2";
        public const string RequestCount = "RequestCount";

        // Təqvim bildirişləri
        public const string ReminderRequestList = "ReminderRequestList";
        public const string CalendarNotificationCount = "CalendarNotificationCount";
        public const string CalendarNotificationList2 = "CalendarNotificationList2";

        // İstifadəçi fəaliyyəti
        public const string RecentActivityUpdate = "RecentActivityUpdate1";
        public const string UpdateUserActivity = "UpdateUserActivity";

        // Dostluq
        public const string InvokeSendFollow = "InwokeSendFollow";
        public const string UpdateMessageFriendList = "UpdateMessageFriendList";
    }
}
