using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.ProjectActivities
{
    public class ProjectActivityAppService : IProjectActivityAppService
    {
        private readonly IProjectActivityService _projectActivityService;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IProjectRealtimeNotifier _notifier;

        public ProjectActivityAppService(
            IProjectActivityService projectActivityService,
            IUserService userService,
            IProjectService projectService,
            IProjectRealtimeNotifier notifier)
        {
            _projectActivityService = projectActivityService;
            _userService = userService;
            _projectService = projectService;
            _notifier = notifier;
        }

        // İstifadəçinin sahibi olduğu bütün layihələrdəki fəaliyyətlər
        public async Task<ServiceResult<List<object>>> GetOwnedProjectsActivitiesAsync(string? userId)
        {
            var activities = await _projectActivityService.GetAll();
            var owned = activities.Where(a => a.Project.CreatedById == userId);

            return await ToActivityItemsAsync(owned, userId);
        }

        public async Task<ServiceResult<List<object>>> GetProjectActivitiesAsync(int projectId, string? userId)
        {
            var activities = await _projectActivityService.GetAllByProjectId(projectId);
            return await ToActivityItemsAsync(activities, userId);
        }

        // Fəaliyyət yazılır və layihənin sahibinə bildirilir
        public async Task<ServiceResult<Empty>> AddAsync(string? userId, ProjectActivityDto dto)
        {
            var activity = dto.ToProjectActivity(userId!);
            var project = await _projectService.GetProjectById(dto.ProjectId);

            await _projectActivityService.Add(activity);
            await _notifier.NotifyProjectActivityAddedAsync(project.CreatedById!);

            return ServiceResult<Empty>.Success(Empty.Value);
        }

        private async Task<ServiceResult<List<object>>> ToActivityItemsAsync(IEnumerable<ProjectActivity> activities, string? userId)
        {
            var currentUser = await _userService.GetUserById(userId!);
            var items = activities.Select(a => a.ToActivityItem(currentUser.UserName)).ToList();

            return ServiceResult<List<object>>.Success(items);
        }
    }
}
