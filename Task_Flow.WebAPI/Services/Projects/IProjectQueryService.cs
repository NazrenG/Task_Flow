using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Projects
{
    /// <summary>
    /// Layihələri oxuyan (state dəyişməyən) əməliyyatlar.
    /// </summary>
    public interface IProjectQueryService
    {
        Task<ServiceResult<object>> GetProjectTitleAsync(int projectId);
        Task<ServiceResult<List<ExtendedProjectListDto>>> GetExtendedProjectListAsync(string userId);
        Task<ServiceResult<int>> GetUserProjectCountAsync(string userId);
        Task<ServiceResult<Project>> GetProjectByTitleAsync(string userId, string title);
        Task<ServiceResult<ProjectDto>> GetProjectAsync(int id);
        Task<ServiceResult<List<CanbanTaskDto>>> GetCanbanTasksAsync(int projectId);

        Task<ServiceResult<List<Project>>> GetOwnProjectsAsync(string? userId);
        Task<ServiceResult<List<Project>>> GetAddedProjectsAsync(string? userId);
        Task<ServiceResult<int?>> GetProjectTaskCountAsync(int id);

        Task<ServiceResult<object>> GetProjectsByStatusAsync(string userId, ProjectStatusFilter status);
        Task<ServiceResult<int>> GetProjectCountByStatusAsync(string userId, ProjectStatusFilter status);

        Task<ServiceResult<object>> GetProjectNamesAsync(string userId);
        Task<ServiceResult<object>> GetTaskChartAsync(string userId, string projectName);
        Task<ServiceResult<List<ExtendedProjectListDto>>> GetInvolvedProjectsAsync(string userId);
    }
}
