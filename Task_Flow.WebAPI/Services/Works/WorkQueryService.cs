using Microsoft.AspNetCore.Identity;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Works
{
    public class WorkQueryService : IWorkQueryService
    {
        private const string UserNotFoundMessage = "User not found.";

        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly UserManager<CustomUser> _userManager;

        public WorkQueryService(
            ITaskService taskService,
            IProjectService projectService,
            IUserService userService,
            UserManager<CustomUser> userManager)
        {
            _taskService = taskService;
            _projectService = projectService;
            _userService = userService;
            _userManager = userManager;
        }

        public async Task<ServiceResult<List<WorkDto>>> GetUserTasksAsync(string userId)
        {
            var tasks = await _taskService.GetTasks(userId);
            return ServiceResult<List<WorkDto>>.Success(tasks.Select(t => t.ToWorkDto(userId)).ToList());
        }

        public async Task<ServiceResult<List<WorkDto>>> GetUserProfileTasksAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ServiceResult<List<WorkDto>>.NotFound("User not found");
            }

            var tasks = await _taskService.GetTasks(user.Id);
            return ServiceResult<List<WorkDto>>.Success(tasks.Select(t => t.ToProfileWorkDto(user.Id)).ToList());
        }

        public async Task<ServiceResult<WorkDto>> GetTaskAsync(int id, string userId)
        {
            var task = await _taskService.GetTaskById(id);
            if (task == null)
            {
                return ServiceResult<WorkDto>.NotFound();
            }

            return ServiceResult<WorkDto>.Success(task.ToSingleWorkDto(userId));
        }

        public async Task<ServiceResult<object>> GetFullWorkDetailAsync(int id)
        {
            var task = await _taskService.GetTaskById(id);
            if (task == null)
            {
                return ServiceResult<object>.NotFound();
            }

            var project = await _projectService.GetProjectById(task.ProjectId);
            var createdBy = await _userService.GetUserById(task.CreatedById);

            return ServiceResult<object>.Success(task.ToFullWorkDetail(project, createdBy));
        }

        public async Task<ServiceResult<int>> GetUserTaskCountAsync(string userId)
        {
            var tasks = await _taskService.GetTasks(userId);
            return ServiceResult<int>.Success(tasks.Count);
        }

        public async Task<ServiceResult<int>> GetUserTaskCountByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ServiceResult<int>.BadRequest(new { message = UserNotFoundMessage });
            }

            return await GetUserTaskCountAsync(user.Id);
        }

        public async Task<ServiceResult<List<Work>>> GetDailyTasksAsync(string userId)
        {
            var tasks = await _taskService.GetTasks(userId);
            var todayTasks = tasks
                .Where(t => t.Deadline.Date == DateTime.Now.Date)
                .OrderBy(t => t.StartTime)
                .ToList();

            return ServiceResult<List<Work>>.Success(todayTasks);
        }

        public async Task<ServiceResult<List<Work>>> GetTasksByStatusAsync(string userId, WorkStatusFilter status)
        {
            return ServiceResult<List<Work>>.Success(await LoadTasksByStatusAsync(userId, status));
        }

        public async Task<ServiceResult<int>> GetTaskCountByStatusAsync(string userId, WorkStatusFilter status)
        {
            var tasks = await LoadTasksByStatusAsync(userId, status);
            return ServiceResult<int>.Success(tasks.Count);
        }

        public async Task<ServiceResult<int>> GetTaskCountByStatusForEmailAsync(string email, WorkStatusFilter status)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ServiceResult<int>.BadRequest(new { message = UserNotFoundMessage });
            }

            return await GetTaskCountByStatusAsync(user.Id, status);
        }

        public async Task<ServiceResult<List<WorkDetailsDto>>> GetProjectOwnerWorksAsync(string userId)
        {
            var works = await _taskService.GetTasksByProjectOwner(userId);
            if (works == null || !works.Any())
            {
                return ServiceResult<List<WorkDetailsDto>>.NotFound("No works found for the current user.");
            }

            return ServiceResult<List<WorkDetailsDto>>.Success(ToDetails(works));
        }

        public async Task<ServiceResult<List<WorkDetailsDto>>> GetProjectWorksAsync(int projectId)
        {
            var works = await _taskService.GetByProjectId(projectId);
            return ServiceResult<List<WorkDetailsDto>>.Success(ToDetails(works));
        }

        public async Task<ServiceResult<List<WorkDetailsDto>>> GetBacklogsAsync(int projectId)
        {
            var works = await _taskService.GetBacklogs(projectId);
            return ServiceResult<List<WorkDetailsDto>>.Success(ToDetails(works));
        }

        private Task<List<Work>> LoadTasksByStatusAsync(string userId, WorkStatusFilter status)
        {
            return status switch
            {
                WorkStatusFilter.ToDo => _taskService.GetToDoTask(userId),
                WorkStatusFilter.InProgress => _taskService.GetInProgressTask(userId),
                WorkStatusFilter.Done => _taskService.GetDoneTask(userId),
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }

        private static List<WorkDetailsDto> ToDetails(IEnumerable<Work>? works)
        {
            return (works ?? Enumerable.Empty<Work>()).Select(w => w.ToWorkDetailsDto()).ToList();
        }
    }
}
