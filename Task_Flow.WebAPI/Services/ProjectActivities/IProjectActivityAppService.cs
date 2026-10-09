using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.ProjectActivities
{
    /// <summary>
    /// Layihələrdəki komanda fəaliyyəti (activity log).
    /// </summary>
    public interface IProjectActivityAppService
    {
        Task<ServiceResult<List<object>>> GetOwnedProjectsActivitiesAsync(string? userId);
        Task<ServiceResult<List<object>>> GetProjectActivitiesAsync(int projectId, string? userId);
        Task<ServiceResult<Empty>> AddAsync(string? userId, ProjectActivityDto dto);
    }
}
