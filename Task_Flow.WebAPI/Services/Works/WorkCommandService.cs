using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Works
{
    public class WorkCommandService : IWorkCommandService
    {
        private const string ProjectRequestType = "ProjectRequest";
        private const string UpdateSuccessMessage = "update succesfuly";
        private const string DeleteSuccessMessage = "delete succesful";

        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IProjectActivityService _projectActivityService;
        private readonly IRequestNotificationService _requestNotificationService;
        private readonly INotificationSettingService _notificationSettingService;
        private readonly IGitHubService _gitHubService;
        private readonly MailService _mailService;
        private readonly IWorkRealtimeNotifier _notifier;

        public WorkCommandService(
            ITaskService taskService,
            IUserService userService,
            IProjectService projectService,
            IProjectActivityService projectActivityService,
            IRequestNotificationService requestNotificationService,
            INotificationSettingService notificationSettingService,
            IGitHubService gitHubService,
            MailService mailService,
            IWorkRealtimeNotifier notifier)
        {
            _taskService = taskService;
            _userService = userService;
            _projectService = projectService;
            _projectActivityService = projectActivityService;
            _requestNotificationService = requestNotificationService;
            _notificationSettingService = notificationSettingService;
            _gitHubService = gitHubService;
            _mailService = mailService;
            _notifier = notifier;
        }

        public async Task<ServiceResult<object>> UpdateTaskStatusAsync(int id, string userId, WorkDto value)
        {
            var task = await _taskService.GetTaskById(id);
            if (task == null)
            {
                return ServiceResult<object>.NotFound();
            }

            task.Priority = value.Priority;
            task.Status = value.Status;
            await _taskService.Update(task);

            await AddProjectActivityAsync(userId, value.ProjectId,
                $"Task updated successfully. New task name: {value.Title}");

            await _notifier.NotifyTaskStatusUpdatedAsync(userId);

            return ServiceResult<object>.Success(new { message = UpdateSuccessMessage });
        }

        public async Task<ServiceResult<object>> UpdateTaskByManagerAsync(int id, string userId, WorkDto value)
        {
            var task = await _taskService.GetTaskById(id);
            if (task == null)
            {
                return ServiceResult<object>.NotFound();
            }

            var project = await _projectService.GetProjectById(value.ProjectId);
            var member = await _userService.GetUserById(value.CreatedById);

            if (project.CreatedById != userId)
            {
                return ServiceResult<object>.BadRequest(new { message = "You are not access edit task" });
            }

            task.ApplyPmEdit(value);
            await _taskService.Update(task);

            var editedMessage = $"Your task edit by {project.CreatedBy?.Firstname} {project.CreatedBy?.Lastname} in the project named {project.Title} ";

            await RealtimeGuard.RunSafelyAsync(async () =>
            {
                await _notifier.NotifyTaskEditedByManagerAsync(userId, member.Id, value.CreatedById!);

                // request getsin taski edit olan sexse
                await _requestNotificationService.Add(new RequestNotification
                {
                    IsAccepted = false,
                    ReceiverId = member.Id,
                    SenderId = userId,
                    NotificationType = ProjectRequestType,
                    ProjectName = project.Title,
                    SentDate = DateTime.UtcNow,
                    Text = editedMessage
                });
                await _notifier.NotifyRequestListsAsync(member.Id);
            });

            // mail getsin taski edit olan sexse
            _mailService.SendEmail(member.Email, editedMessage);

            await AddProjectActivityAsync(userId, value.ProjectId,
                $"The task named '{value.Title}' has been successfully updated for {member.Firstname} {member.Lastname}.");

            return ServiceResult<object>.Success(new { message = UpdateSuccessMessage });
        }

        public async Task<ServiceResult<Work>> CreateTaskAsync(string userId, WorkDto value)
        {
            var member = await _userService.GetUserById(value.CreatedById);
            var project = await _projectService.GetProjectById(value.ProjectId);
            if (project == null)
            {
                return ServiceResult<Work>.NotFound("Project not found");
            }

            if (member == null || string.IsNullOrEmpty(member.GitHubAccessToken))
            {
                return ServiceResult<Work>.BadRequest("İstifadəçi GitHub hesabını qoşmalıdır");
            }

            var branchName = BuildBranchName(value.Title);

            // Branch ASSIGNED USER-in token-i ilə yaradılır (onlar öz branch-larını yaradır)
            var branchCreated = await _gitHubService.CreateBranch(
                member.GitHubAccessToken,
                project.CreatedBy.GitHubUsername,
                project.GitHubRepositoryName,
                branchName);

            if (!branchCreated)
            {
                return ServiceResult<Work>.Failure("Branch yaradıla bilmədi");
            }

            var task = value.ToNewWork(branchName);
            await _taskService.Add(task);

            await RealtimeGuard.RunSafelyAsync(() =>
                _notifier.NotifyTaskCreatedAsync(userId, member.Id, value.CreatedById!));

            await _requestNotificationService.Add(new RequestNotification
            {
                ReceiverId = value.CreatedById,
                SenderId = userId,
                Text = $"You have a new task({value.Title}) in the project named {project.Title}",
                IsAccepted = false,
                NotificationType = ProjectRequestType,
                ProjectName = project.Title
            });
            // notification list project taski ucun
            await _notifier.NotifyRequestListsAsync(member.Id);

            await AddProjectActivityAsync(userId, value.ProjectId,
                $"A new task named '{value.Title}' has been created for {member.Firstname} {member.Lastname}.");
            await _notifier.NotifyProjectActivityAsync(member.Id, userId);

            await SendNewTaskMailIfAllowedAsync(userId, member, project);

            return ServiceResult<Work>.Success(task);
        }

        public async Task<ServiceResult<object>> DeleteTaskAsync(int taskId, int projectId, string userId)
        {
            var task = await _taskService.GetTaskById(taskId);
            var project = await _projectService.GetProjectById(projectId);

            if (project.CreatedById != userId)
            {
                return ServiceResult<object>.BadRequest("You do not have permission to delete tasks in this project.");
            }

            if (task == null)
            {
                return ServiceResult<object>.NotFound();
            }

            await _taskService.Delete(task);

            await RealtimeGuard.RunSafelyAsync(() =>
                _notifier.NotifyTaskDeletedAsync(userId, task.CreatedById!));

            await AddProjectActivityAsync(userId, projectId,
                $"{task.CreatedBy?.Firstname} {task.CreatedBy?.Lastname}`s task delete successfully. New task name:{task.Title} ");

            await _requestNotificationService.Add(new RequestNotification
            {
                ReceiverId = task.CreatedById,
                SenderId = userId,
                Text = $"The task '{task.Title}' has been deleted by the project manager in the project '{project.Title}'.",
                IsAccepted = false,
                NotificationType = ProjectRequestType,
                ProjectName = project.Title
            });
            // notification list project taski ucun
            await _notifier.NotifyRequestListsAsync(task.CreatedById!);

            // mail getsin taski silinen sexse
            _mailService.SendEmail(task.CreatedBy?.Email,
                $"Your task'{task.Title}' has been deleted by the project manager in the project '{project.Title}' ");

            return ServiceResult<object>.Success(new { message = DeleteSuccessMessage });
        }

        private static string BuildBranchName(string? title)
        {
            return $"task/{title!.ToLower().Replace(" ", "-")}";
        }

        // yeni task yaradilanda eger icaze varsa maile mesaj getsin
        private async Task SendNewTaskMailIfAllowedAsync(string userId, CustomUser member, Project project)
        {
            var notificationSetting = await _notificationSettingService.GetNotificationSetting(userId);
            if (notificationSetting != null && notificationSetting.NewTaskWithInProject)
            {
                _mailService.SendEmail(member.Email,
                    $"Hi,{member.Firstname} {member.Lastname}.You have a new task in the project named {project.Title} ");
            }
        }

        private Task AddProjectActivityAsync(string userId, int projectId, string text)
        {
            return _projectActivityService.Add(new ProjectActivity
            {
                UserId = userId,
                ProjectId = projectId,
                Text = text
            });
        }
    }
}
