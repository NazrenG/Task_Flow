using Microsoft.AspNetCore.Identity;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class CustomUser:IdentityUser,IEntity
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; } 
        public string? Occupation { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Image { get; set; }
        public string? Gender { get; set; }
        public string? GitHubAccessToken { get; set; }
        public string? GitHubUsername { get; set; }

        public bool? IsOnline { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? RegisterDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
   

        // Navigation properties
        public virtual List<Project>? Projects { get; set; }
        public virtual List<Work>? TaskForUsers { get; set; }
        public virtual List<TeamMember>? TeamMembers { get; set; }
        public virtual List<Comment>? Comments { get; set; }
        public virtual List<TaskAssigne>? TaskAssignees { get; set; }
        public virtual List<Message>? MessagesSender { get; set; }
        public virtual List<Message>? MessagesReceiver { get; set; }
        public virtual List<Friend>? Friends { get; set; }
        public virtual List<Friend>? FriendsOf { get; set; }
        public virtual List<Notification>? Notifications { get; set; }
        public virtual List<RecentActivity>? RecentActivities { get; set; }
        public virtual List<RequestNotification> RequestNotificationsSender { get; set; }
        public virtual List<RequestNotification> RequestNotificationsReceiver { get; set; }
        public virtual ICollection<Chat>? Chats { get; set; }
        public virtual List<UserTask> UserTasks { get; set; }

        public virtual NotificationSetting Setting { get; set; }
        public CustomUser()
        {
             RegisterDate = DateTime.UtcNow;    
            Chats = new List<Chat>();
        }


    }
}
