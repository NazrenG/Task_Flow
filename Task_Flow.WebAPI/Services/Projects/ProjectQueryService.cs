using System.Globalization;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Projects
{
    public class ProjectQueryService : IProjectQueryService
    {
        private const int ChartMonthCount = 6;

        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly ITaskService _taskService;
        private readonly ITeamMemberService _teamMemberService;
        private readonly ICompanyService _companyService;

        public ProjectQueryService(
            IProjectService projectService,
            IUserService userService,
            ITaskService taskService,
            ITeamMemberService teamMemberService,
            ICompanyService companyService)
        {
            _projectService = projectService;
            _userService = userService;
            _taskService = taskService;
            _teamMemberService = teamMemberService;
            _companyService = companyService;
        }

        public async Task<ServiceResult<object>> GetProjectTitleAsync(int projectId)
        {
            var project = await _projectService.GetProjectById(projectId);
            if (project == null)
            {
                return ServiceResult<object>.NotFound();
            }

            return ServiceResult<object>.Success(new { Title = project.Title, Color = project.Color });
        }

        public async Task<ServiceResult<List<ExtendedProjectListDto>>> GetExtendedProjectListAsync(string userId)
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<List<ExtendedProjectListDto>>.NotFound("User not found.");
            }

            var projects = await _projectService.GetProjects(userId);
            return ServiceResult<List<ExtendedProjectListDto>>.Success(
                projects.Select(p => p.ToExtendedProjectListDto()).ToList());
        }

        public async Task<ServiceResult<int>> GetUserProjectCountAsync(string userId)
        {
            return ServiceResult<int>.Success(await _projectService.GetUserProjectCount(userId));
        }

        public async Task<ServiceResult<Project>> GetProjectByTitleAsync(string userId, string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return ServiceResult<Project>.BadRequest(new { message = "Title is required." });
            }

            var project = await _projectService.GetProjectByName(userId, title);
            if (project == null)
            {
                return ServiceResult<Project>.NotFound(new { message = "Project not found." });
            }

            return ServiceResult<Project>.Success(project);
        }

        public async Task<ServiceResult<ProjectDto>> GetProjectAsync(int id)
        {
            var project = await _projectService.GetProjectById(id);
            if (project == null)
            {
                return ServiceResult<ProjectDto>.NotFound();
            }

            var dto = project.ToProjectDto(await _companyService.IsCompanyProject(id));
            var (memberUsernames, membersPath) = await GetTeamMemberInfoAsync(id);
            dto.Members = memberUsernames;
            dto.MembersPath = membersPath;

            return ServiceResult<ProjectDto>.Success(dto);
        }

        public async Task<ServiceResult<List<CanbanTaskDto>>> GetCanbanTasksAsync(int projectId)
        {
            var project = await _projectService.GetProjectById(projectId);
            if (project == null)
            {
                return ServiceResult<List<CanbanTaskDto>>.NotFound();
            }

            var tasks = await _taskService.GetByProjectId(projectId);
            return ServiceResult<List<CanbanTaskDto>>.Success(tasks.Select(t => t.ToCanbanTaskDto()).ToList());
        }

        public async Task<ServiceResult<List<Project>>> GetOwnProjectsAsync(string? userId)
        {
            return ServiceResult<List<Project>>.Success(await _projectService.GetProjects(userId!));
        }

        public async Task<ServiceResult<List<Project>>> GetAddedProjectsAsync(string? userId)
        {
            var memberships = await _teamMemberService.GetProjectListByUserIdAsync(userId!);
            var projects = new List<Project>();
            foreach (var membership in memberships)
            {
                projects.Add(await _projectService.GetProjectById(membership.ProjectId));
            }

            return ServiceResult<List<Project>>.Success(projects);
        }

        public async Task<ServiceResult<int?>> GetProjectTaskCountAsync(int id)
        {
            var project = await _projectService.GetProjectWithTasksById(id);
            if (project == null)
            {
                return ServiceResult<int?>.NotFound();
            }

            return ServiceResult<int?>.Success(project.TaskForUsers?.Count);
        }

        public async Task<ServiceResult<object>> GetProjectsByStatusAsync(string userId, ProjectStatusFilter status)
        {
            var projects = await LoadProjectsByStatusAsync(userId, status);

            // On Going layihələri dashboard üçün qısaldılmış formada qaytarılır
            object result = status == ProjectStatusFilter.OnGoing
                ? projects.Select(p => p.ToOnGoingProjectItem())
                : projects;

            return ServiceResult<object>.Success(result);
        }

        public async Task<ServiceResult<int>> GetProjectCountByStatusAsync(string userId, ProjectStatusFilter status)
        {
            var projects = await LoadProjectsByStatusAsync(userId, status);
            return ServiceResult<int>.Success(projects.Count);
        }

        public async Task<ServiceResult<object>> GetProjectNamesAsync(string userId)
        {
            var projects = await _projectService.GetProjects(userId);
            var names = projects.Select(p => p.Title).ToList();
            return ServiceResult<object>.Success(new { Names = names });
        }

        public async Task<ServiceResult<object>> GetTaskChartAsync(string userId, string projectName)
        {
            var project = await _projectService.GetProjectByName(userId, projectName);

            var completedTasks = new List<int>();
            var onGoingTasks = new List<int>();
            var currentDate = DateTime.Now;
            var year = DateTime.UtcNow.Year;

            for (int i = 0; i < ChartMonthCount; i++)
            {
                int monthIndex = (currentDate.Month - i + 12) % 12;
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthIndex + 1);
                var summary = await _taskService.GetTaskSummaryByMonthAsync(project.Id, monthIndex, year);

                completedTasks.Add(summary[0]);
                onGoingTasks.Add(summary[1]);

                if (monthName == "January") year--;
            }

            completedTasks.Reverse();
            onGoingTasks.Reverse();

            return ServiceResult<object>.Success(new { Complated = completedTasks, OnGoing = onGoingTasks });
        }

        public async Task<ServiceResult<List<ExtendedProjectListDto>>> GetInvolvedProjectsAsync(string userId)
        {
            var memberships = await _teamMemberService.GetProjectListByUserIdAsync(userId);
            var projects = new List<ExtendedProjectListDto>();

            foreach (var membership in memberships)
            {
                var project = await _projectService.GetProjectById(membership.ProjectId);
                projects.Add(project.ToInvolvedProjectDto());
            }

            return ServiceResult<List<ExtendedProjectListDto>>.Success(projects);
        }

        private Task<List<Project>> LoadProjectsByStatusAsync(string userId, ProjectStatusFilter status)
        {
            return status switch
            {
                ProjectStatusFilter.OnGoing => _projectService.GetOnGoingProject(userId),
                ProjectStatusFilter.Pending => _projectService.GetPendingProject(userId),
                ProjectStatusFilter.Completed => _projectService.GetCompletedTask(userId),
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }

        private async Task<(List<string> Usernames, List<string> ImagePaths)> GetTeamMemberInfoAsync(int projectId)
        {
            var teamMembers = await _teamMemberService.GetTaskMemberListById(projectId);
            var usernames = new List<string>();
            var imagePaths = new List<string>();

            foreach (var teamMember in teamMembers)
            {
                var user = await _userService.GetUserById(teamMember.UserId!);
                if (user != null)
                {
                    usernames.Add(user.UserName!);
                    imagePaths.Add(user.Image!);
                }
            }

            return (usernames, imagePaths);
        }
    }
}
