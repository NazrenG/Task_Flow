using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Services.Mapping
{
    public static class ProjectActivityMappingExtensions
    {
        private const string CurrentUserLabel = "You";

        // Fəaliyyəti cari istifadəçi edibsə, ad əvəzinə "You" göstərilir
        public static object ToActivityItem(this ProjectActivity activity, string? currentUsername)
        {
            return new
            {
                Username = currentUsername == activity.User.UserName
                    ? CurrentUserLabel
                    : $"{activity.User.Firstname} {activity.User.Lastname}",
                ProjectName = activity.Project.Title,
                CreateDate = activity.CreateTime,
                Text = activity.Text,
                Path = activity.User.Image
            };
        }

        public static ProjectActivity ToProjectActivity(this ProjectActivityDto dto, string userId)
        {
            return new ProjectActivity
            {
                Text = dto.Text,
                UserId = userId,
                CreateTime = DateTime.UtcNow,
                ProjectId = dto.ProjectId
            };
        }
    }
}
