using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Services.Mapping
{
    public static class NotificationMappingExtensions
    {
        public static object ToSenderSummary(this RequestNotification request)
        {
            return new
            {
                Text = request.Text,
                Username = request.Sender?.UserName,
                Path = request.Sender!.Image
            };
        }

        public static object ToRequestItem(this RequestNotification request)
        {
            return new
            {
                RequestId = request.Id,
                Text = request.Text,
                SenderName = $"{request.Sender!.Firstname} {request.Sender.Lastname}",
                Image = request.Sender.Image,
                Typee = request.NotificationType
            };
        }

        public static object ToCalendarItem(this Notification notification)
        {
            return new
            {
                Id = notification.Id,
                Text = notification.Text,
                Date = notification.Created
            };
        }

        public static object ToCalendarSummary(this Notification notification)
        {
            return new
            {
                Text = notification.Text,
                Username = notification.User?.UserName
            };
        }

        public static RecentActivityDto ToRecentActivityDto(this RecentActivity activity)
        {
            return new RecentActivityDto
            {
                Text = activity.Text,
                Type = activity.Type,
                Created = activity.Created
            };
        }

        public static void ApplyTo(this NotificationSettingDto dto, NotificationSetting setting)
        {
            setting.NewTaskWithInProject = dto.NewTaskWithInProject;
            setting.FriendshipOffers = dto.FriendshipOffers;
            setting.ProjectCompletationDate = dto.ProjectCompletationDate;
            setting.InnovationNewProject = dto.InnovationNewProject;
            setting.TaskDueDate = dto.TaskDueDate;
        }
    }
}
