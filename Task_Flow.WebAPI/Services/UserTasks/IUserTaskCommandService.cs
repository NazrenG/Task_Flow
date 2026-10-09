using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.UserTasks
{
    /// <summary>
    /// İstifadəçinin şəxsi task-larını yaradan, redaktə edən və silən əməliyyatlar.
    /// </summary>
    public interface IUserTaskCommandService
    {
        Task<ServiceResult<UserTask>> CreateTaskAsync(string userId, WorkDto value);
        Task<ServiceResult<object>> EditTaskAsync(int id, string userId, WorkDto value);
        Task<ServiceResult<object>> EditTaskForCalendarAsync(int id, string userId, EditForCalendarDto value);
        Task<ServiceResult<object>> DeleteTaskAsync(int taskId, string userId);
    }
}
