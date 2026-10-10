using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.TeamMembers
{
    public class TeamMemberCommandService : ITeamMemberCommandService
    {
        private readonly ITeamMemberService _teamMemberService;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IGitHubService _gitHubService;
        private readonly MailService _mailService;
        private readonly IProjectRealtimeNotifier _projectNotifier;

        public TeamMemberCommandService(
            ITeamMemberService teamMemberService,
            IUserService userService,
            IProjectService projectService,
            IGitHubService gitHubService,
            MailService mailService,
            IProjectRealtimeNotifier projectNotifier)
        {
            _teamMemberService = teamMemberService;
            _userService = userService;
            _projectService = projectService;
            _gitHubService = gitHubService;
            _mailService = mailService;
            _projectNotifier = projectNotifier;
        }

        // Üzv əvvəlcə GitHub repo-suna collaborator kimi əlavə olunur, sonra komandaya
        public async Task<ServiceResult<TeamMember>> AddMemberAsync(TeamMemberDto value)
        {
            var user = await _userService.GetUserById(value.UserId!);
            if (string.IsNullOrEmpty(user.GitHubUsername))
            {
                return ServiceResult<TeamMember>.BadRequest("İstifadəçi GitHub hesabını qoşmalıdır");
            }

            var project = await _projectService.GetProjectById(value.ProjectId);
            var addedToGitHub = await _gitHubService.AddCollaborator(
                project.CreatedBy!.GitHubAccessToken!,
                project.CreatedBy.GitHubUsername!,
                project.GitHubRepositoryName!,
                user.GitHubUsername);

            if (!addedToGitHub)
            {
                return ServiceResult<TeamMember>.Failure("GitHub-da əlavə edilə bilmədi");
            }

            var member = new TeamMember
            {
                ProjectId = value.ProjectId,
                UserId = value.UserId,
                GitHubAccessGranted = true
            };
            await _teamMemberService.Add(member);

            return ServiceResult<TeamMember>.Success(member);
        }

        public async Task<ServiceResult<Empty>> ChangeMemberUserAsync(int id, string userId)
        {
            var member = await _teamMemberService.GetTaskMemberById(id);
            if (member == null)
            {
                return ServiceResult<Empty>.NotFound();
            }

            member.UserId = userId;
            await _teamMemberService.Update(member);

            return ServiceResult<Empty>.Success(Empty.Value);
        }

        public async Task<ServiceResult<Empty>> DeleteAsync(int id)
        {
            var member = await _teamMemberService.GetTaskMemberById(id);
            if (member == null)
            {
                return ServiceResult<Empty>.NotFound();
            }

            await _teamMemberService.Delete(member);

            return ServiceResult<Empty>.Success(Empty.Value);
        }

        public async Task<ServiceResult<Empty>> RemoveFromProjectAsync(string? currentUserId, RemoveMemberDto dto)
        {
            var user = await _userService.GetOneUSerByUsername(dto.Username);

            await _teamMemberService.DeleteTeamMemberAsync(dto.ProjectId, user.Id);

            var project = await _projectService.GetProjectById(dto.ProjectId);
            _mailService.SendEmail(user.Email!,
                "You were removed from project " + project.Title + " at " + DateTime.UtcNow.ToShortDateString() + " by PM");

            await _projectNotifier.NotifyProjectMembersChangedAsync(currentUserId!);

            return ServiceResult<Empty>.Success(Empty.Value);
        }
    }
}
