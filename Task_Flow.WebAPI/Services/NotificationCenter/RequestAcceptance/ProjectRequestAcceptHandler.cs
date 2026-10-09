using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.WebAPI.Services.NotificationCenter.RequestAcceptance
{
    // Layihəyə dəvət qəbul edildikdə istifadəçi layihənin komanda üzvü olur
    public class ProjectRequestAcceptHandler : IRequestAcceptHandler
    {
        private readonly IProjectService _projectService;
        private readonly ITeamMemberService _teamMemberService;

        public ProjectRequestAcceptHandler(IProjectService projectService, ITeamMemberService teamMemberService)
        {
            _projectService = projectService;
            _teamMemberService = teamMemberService;
        }

        public string NotificationType => RequestNotificationTypes.ProjectRequest;

        public async Task HandleAsync(RequestNotification request, string userId)
        {
            var project = await _projectService.GetProjectByName(request.SenderId!, request.ProjectName!);
            await _teamMemberService.Add(new TeamMember
            {
                ProjectId = project.Id,
                UserId = request.ReceiverId
            });
        }
    }
}
