using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Works
{
    /// <summary>
    /// Task-ın state-ini dəyişən əməliyyatlar (yaratma, redaktə, silmə).
    /// </summary>
    public interface IWorkCommandService
    {
        Task<ServiceResult<object>> UpdateTaskStatusAsync(int id, string userId, WorkDto value);
        Task<ServiceResult<object>> UpdateTaskByManagerAsync(int id, string userId, WorkDto value);
        Task<ServiceResult<Work>> CreateTaskAsync(string userId, WorkDto value);
        Task<ServiceResult<object>> DeleteTaskAsync(int taskId, int projectId, string userId);
    }
}
