using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.NotificationCenter;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.TeamMembers
{
    public class TeamInvitationService : ITeamInvitationService
    {
        private const string SuccessMessage = "Team members added successfully!";

        private readonly IUserService _userService;
        private readonly ITeamMemberService _teamMemberService;
        private readonly IProjectService _projectService;
        private readonly IRequestNotificationService _requestNotificationService;
        private readonly INotificationSettingService _notificationSettingService;
        private readonly IPremiumUserService _premiumUserService;
        private readonly MailService _mailService;
        private readonly INotificationRealtimeNotifier _notifier;

        public TeamInvitationService(
            IUserService userService,
            ITeamMemberService teamMemberService,
            IProjectService projectService,
            IRequestNotificationService requestNotificationService,
            INotificationSettingService notificationSettingService,
            IPremiumUserService premiumUserService,
            MailService mailService,
            INotificationRealtimeNotifier notifier)
        {
            _userService = userService;
            _teamMemberService = teamMemberService;
            _projectService = projectService;
            _requestNotificationService = requestNotificationService;
            _notificationSettingService = notificationSettingService;
            _premiumUserService = premiumUserService;
            _mailService = mailService;
            _notifier = notifier;
        }

        public async Task<ServiceResult<object>> InviteMembersAsync(string? senderId, TeamMemberCollectionDto dto)
        {
            var sender = await _userService.GetUserById(senderId!);
            if (!HasMembers(dto))
            {
                return NoMember();
            }

            foreach (var username in dto.Members)
            {
                var user = await _userService.GetOneUSerByUsername(username);
                if (user == null)
                {
                    return UserNotFound(username);
                }

                var projectName = await _projectService.GetProjectNameById(dto.ProjectId);
                await SendInvitationAsync(senderId, sender, user, projectName);

                _mailService.SendEmail(user.Email!,
                    sender.Firstname + " " + sender.Lastname + " invited you to their project " + projectName);
            }

            return ServiceResult<object>.Success(new { Message = SuccessMessage });
        }

        // Layihənin mövcud komandası silinir və siyahıdakı istifadəçilərə yeni dəvət göndərilir
        public async Task<ServiceResult<object>> ReplaceTeamAndInviteAsync(string? senderId, TeamMemberCollectionDto dto)
        {
            if (!HasMembers(dto))
            {
                return NoMember();
            }

            var sender = await _userService.GetUserById(senderId!);
            var currentMembers = await _teamMemberService.GetTaskMemberListById(dto.ProjectId);
            await _teamMemberService.RemoveMembers(currentMembers);

            var permission = await _premiumUserService.IsUserAllowedToAddTeammember(currentMembers.Count, senderId!);
            if (!permission.Allowed)
            {
                return ServiceResult<object>.Success(new { message = permission.Message });
            }

            foreach (var username in dto.Members)
            {
                var user = await _userService.GetOneUSerByUsername(username);
                var projectName = await _projectService.GetProjectNameById(dto.ProjectId);
                if (user == null)
                {
                    return UserNotFound(username);
                }

                await SendInvitationAsync(senderId, sender, user, projectName);

                // proyektde istirak ucun eger icaze varsa mail gedir
                var notificationSetting = await _notificationSettingService.GetNotificationSetting(user.Id);
                if (notificationSetting.NewTaskWithInProject)
                {
                    _mailService.SendEmail(user.Email!,
                        $"Hi,{user.Firstname} {user.Lastname}.You have a new task in the project named {projectName} ");
                }
            }

            return ServiceResult<object>.Success(new { Message = SuccessMessage });
        }

        public async Task<ServiceResult<object>> CancelInvitationAsync(RemoveTeamMemberDto dto)
        {
            var invitations = await _requestNotificationService.GetNotificationsByProjectName(dto.Title);
            var invitation = invitations.FirstOrDefault(n => n.ReceiverId == dto.RecieverId);
            if (invitation == null)
            {
                return ServiceResult<object>.Success(new { Code = 404 });
            }

            await _requestNotificationService.Delete(invitation);

            return ServiceResult<object>.Success(new { Code = 200 });
        }

        private async Task SendInvitationAsync(string? senderId, CustomUser sender, CustomUser receiver, string projectName)
        {
            await _requestNotificationService.Add(new RequestNotification
            {
                IsAccepted = false,
                ReceiverId = receiver.Id,
                SenderId = senderId,
                NotificationType = RequestNotificationTypes.ProjectRequest,
                ProjectName = projectName,
                SentDate = DateTime.UtcNow,
                Text = "Hi, I am " + sender.Firstname + " " + sender.Lastname + ". I want to invite you to my project named: " + projectName
            });

            // notification list
            await _notifier.NotifyRequestListsAsync(receiver.Id);
        }

        private static bool HasMembers(TeamMemberCollectionDto? dto)
        {
            return dto != null && dto.Members != null && dto.Members.Any();
        }

        private static ServiceResult<object> NoMember()
        {
            return ServiceResult<object>.Success(new { Message = "No member" });
        }

        private static ServiceResult<object> UserNotFound(string username)
        {
            return ServiceResult<object>.NotFound(new { Message = $"User '{username}' not found." });
        }
    }
}
