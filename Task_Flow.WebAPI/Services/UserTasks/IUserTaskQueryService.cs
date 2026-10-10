using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;
using Task_Flow.WebAPI.Services.Works;

namespace Task_Flow.WebAPI.Services.UserTasks
{
    /// <summary>
    /// İstifadəçinin şəxsi task-larını oxuyan əməliyyatlar.
    /// </summary>
    public interface IUserTaskQueryService
    {
        Task<ServiceResult<List<WorkDto>>> GetUserTasksAsync(string userId);
        Task<ServiceResult<WorkDto>> GetUserTaskAsync(int id, string userId);
        Task<ServiceResult<int>> GetUserTaskCountAsync(string userId);
        Task<ServiceResult<List<UserTask>>> GetDailyTasksAsync(string userId);

        Task<ServiceResult<List<UserTask>>> GetTasksByStatusAsync(string userId, WorkStatusFilter status);
        Task<ServiceResult<int>> GetTaskCountByStatusAsync(string userId, WorkStatusFilter status);
        Task<ServiceResult<int>> GetTaskCountByStatusForEmailAsync(string email, WorkStatusFilter status);
    }
}
