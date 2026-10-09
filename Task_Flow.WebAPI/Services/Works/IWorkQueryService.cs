using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Works
{
    /// <summary>
    /// Task-ları oxuyan (state dəyişməyən) əməliyyatlar.
    /// </summary>
    public interface IWorkQueryService
    {
        Task<ServiceResult<List<WorkDto>>> GetUserTasksAsync(string userId);
        Task<ServiceResult<List<WorkDto>>> GetUserProfileTasksAsync(string email);
        Task<ServiceResult<WorkDto>> GetTaskAsync(int id, string userId);
        Task<ServiceResult<object>> GetFullWorkDetailAsync(int id);

        Task<ServiceResult<int>> GetUserTaskCountAsync(string userId);
        Task<ServiceResult<int>> GetUserTaskCountByEmailAsync(string email);

        Task<ServiceResult<List<Work>>> GetDailyTasksAsync(string userId);
        Task<ServiceResult<List<Work>>> GetTasksByStatusAsync(string userId, WorkStatusFilter status);
        Task<ServiceResult<int>> GetTaskCountByStatusAsync(string userId, WorkStatusFilter status);
        Task<ServiceResult<int>> GetTaskCountByStatusForEmailAsync(string email, WorkStatusFilter status);

        Task<ServiceResult<List<WorkDetailsDto>>> GetProjectOwnerWorksAsync(string userId);
        Task<ServiceResult<List<WorkDetailsDto>>> GetProjectWorksAsync(int projectId);
        Task<ServiceResult<List<WorkDetailsDto>>> GetBacklogsAsync(int projectId);
    }
}
