using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Projects
{
    public class ProjectCommandService : IProjectCommandService
    {
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly ITaskService _taskService;
        private readonly IProjectActivityService _projectActivityService;
        private readonly IGitHubService _gitHubService;
        private readonly IPremiumUserService _premiumUserService;
        private readonly ICompanyService _companyService;
        private readonly IProjectRealtimeNotifier _projectNotifier;
        private readonly IWorkRealtimeNotifier _workNotifier;

        public ProjectCommandService(
            IProjectService projectService,
            IUserService userService,
            ITaskService taskService,
            IProjectActivityService projectActivityService,
            IGitHubService gitHubService,
            IPremiumUserService premiumUserService,
            ICompanyService companyService,
            IProjectRealtimeNotifier projectNotifier,
            IWorkRealtimeNotifier workNotifier)
        {
            _projectService = projectService;
            _userService = userService;
            _taskService = taskService;
            _projectActivityService = projectActivityService;
            _gitHubService = gitHubService;
            _premiumUserService = premiumUserService;
            _companyService = companyService;
            _projectNotifier = projectNotifier;
            _workNotifier = workNotifier;
        }

        public async Task<ServiceResult<object>> CreateProjectAsync(string? userId, CreateProjectDto value)
        {
            var user = await _userService.GetUserById(userId!);
            await _gitHubService.CreateRepository(user.GitHubAccessToken!, value.Title!, value.Description!);

            var project = value.ToNewProject(userId!);
            if (value.IsCompanyProject)
            {
                var company = await _companyService.GetCompany(userId!);
                project.CompanyId = company.CompanyId;
            }

            var permission = await _premiumUserService.IsUserAllowedToCreateProjectAsync(userId!, project);
            if (!permission.Allowed)
            {
                return ServiceResult<object>.Success(new { message = permission.Message, allowed = false });
            }

            await _projectService.Add(project);

            await AddProjectActivityAsync(userId, project.Id, "created a new Project named: " + project.Title);
            await _projectNotifier.NotifyProjectCreatedAsync(userId!, value.Status);

            return ServiceResult<object>.Success(new { item = project, allowed = true });
        }

        public async Task<ServiceResult<Empty>> UpdateProjectAsync(int id, string? userId, PutProjectDto dto)
        {
            var project = await _projectService.GetProjectById(id);
            if (project == null)
            {
                return ServiceResult<Empty>.NotFound();
            }

            project.ApplyUpdate(dto);
            await _projectService.Update(project);

            await AddProjectActivityAsync(userId, id, "Changed Project Title to: " + project.Title);
            await _projectNotifier.NotifyProjectUpdatedAsync(userId!);

            return ServiceResult<Empty>.Success(Empty.Value);
        }

        public Task<ServiceResult<Empty>> ChangeTitleAsync(int id, string? userId, string value)
        {
            return UpdateFieldAsync(id, userId,
                project => project.Title = value,
                project => "Changed Project Title to: " + project.Title);
        }

        public Task<ServiceResult<Empty>> ChangeDescriptionAsync(int id, string? userId, string value)
        {
            return UpdateFieldAsync(id, userId,
                project => project.Description = value,
                _ => "Changed Project Description");
        }

        public Task<ServiceResult<Empty>> ChangeCompletedAsync(int id, string? userId, bool value)
        {
            return UpdateFieldAsync(id, userId,
                project => project.IsCompleted = value,
                _ => "Project Completed");
        }

        public async Task<ServiceResult<Project>> DeleteProjectAsync(int id, string? userId)
        {
            var project = await _projectService.GetProjectById(id);
            if (project == null)
            {
                return ServiceResult<Project>.NotFound();
            }

            await AddProjectActivityAsync(userId, id, "Project (" + project.Title + ") Deleted!");
            await _projectService.Delete(project);
            await _projectNotifier.NotifyProjectDeletedAsync(userId!);

            return ServiceResult<Project>.Success(project);
        }

        // canbanda tasklarin statusunu deyisdirmek
        public async Task<ServiceResult<string>> UpdateTaskColumnAsync(string? userId, UpdateTaskColumnDto dto)
        {
            var task = await _taskService.GetTaskById(dto.TaskId);
            if (task == null)
            {
                return ServiceResult<string>.NotFound("Task not found.");
            }

            var project = await _projectService.GetProjectById(task.ProjectId);
            if (project.CreatedById != userId)
            {
                return ServiceResult<string>.BadRequest("You do not have permission to update tasks in this project.");
            }

            task.CanbanColumnId = dto.NewCanbanColumnId;
            task.Status = dto.Status;
            await _taskService.Update(task);

            await RealtimeGuard.RunSafelyAsync(() =>
                _workNotifier.NotifyTaskColumnChangedAsync(userId!, task.CreatedById!));

            await AddProjectActivityAsync(userId, task.ProjectId, $"Task '{task.Title}' moved to another column.");

            return ServiceResult<string>.Success("Task column updated successfully.");
        }

        private async Task<ServiceResult<Empty>> UpdateFieldAsync(
            int id,
            string? userId,
            Action<Project> applyChange,
            Func<Project, string> activityText)
        {
            var project = await _projectService.GetProjectById(id);
            if (project == null)
            {
                return ServiceResult<Empty>.NotFound();
            }

            applyChange(project);
            await _projectService.Update(project);
            await AddProjectActivityAsync(userId, id, activityText(project));

            return ServiceResult<Empty>.Success(Empty.Value);
        }

        private Task AddProjectActivityAsync(string? userId, int projectId, string text)
        {
            return _projectActivityService.Add(new ProjectActivity
            {
                UserId = userId!,
                ProjectId = projectId,
                Text = text
            });
        }
    }
}
