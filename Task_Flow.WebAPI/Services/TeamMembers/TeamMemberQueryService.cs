using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.TeamMembers
{
    public class TeamMemberQueryService : ITeamMemberQueryService
    {
        private readonly ITeamMemberService _teamMemberService;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IRequestNotificationService _requestNotificationService;

        public TeamMemberQueryService(
            ITeamMemberService teamMemberService,
            IUserService userService,
            IProjectService projectService,
            IRequestNotificationService requestNotificationService)
        {
            _teamMemberService = teamMemberService;
            _userService = userService;
            _projectService = projectService;
            _requestNotificationService = requestNotificationService;
        }

        public async Task<List<TeamMemberDto>> GetAllAsync()
        {
            var members = await _teamMemberService.TeamMembers();
            return members.Select(m => m.ToTeamMemberDto()).ToList();
        }

        public async Task<ServiceResult<TeamMemberDto>> GetByIdAsync(int id)
        {
            var member = await _teamMemberService.GetTaskMemberById(id);
            if (member == null)
            {
                return ServiceResult<TeamMemberDto>.NotFound();
            }

            return ServiceResult<TeamMemberDto>.Success(member.ToTeamMemberDto());
        }

        public async Task<ServiceResult<List<TeamUserDto>>> GetUsersByProjectAsync(int projectId)
        {
            var members = await _teamMemberService.GetTaskMemberListById(projectId);
            return ServiceResult<List<TeamUserDto>>.Success(members.Select(m => m.ToTeamUserDto()).ToList());
        }

        // Komanda üzvləri və layihəyə göndərilmiş dəvətlər bir siyahıda qaytarılır
        public async Task<ServiceResult<object>> GetMembersWithInvitationsAsync(int projectId)
        {
            var result = new List<ExtendedTeamMemberDto>();

            var members = await _teamMemberService.GetTaskMemberListById(projectId);
            foreach (var member in members)
            {
                var user = await _userService.GetUserById(member.UserId!);
                result.Add(user.ToExtendedTeamMemberDto(isRequest: false, isAccepted: true));
            }

            var project = await _projectService.GetProjectById(projectId);
            var invitations = await _requestNotificationService.GetNotificationsByProjectName(project.Title!);
            foreach (var invitation in invitations)
            {
                var user = await _userService.GetUserById(invitation.ReceiverId!);
                result.Add(user.ToExtendedTeamMemberDto(isRequest: true, isAccepted: invitation.IsAccepted));
            }

            return ServiceResult<object>.Success(new { List = result });
        }
    }
}
