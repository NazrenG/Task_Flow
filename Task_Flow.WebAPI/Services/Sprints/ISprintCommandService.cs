using Task_Flow.Business.DTOs;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Sprints
{
    /// <summary>
    /// Sprint yaratmaq, silmək və task-ları sprint-lərə bağlamaq.
    /// </summary>
    public interface ISprintCommandService
    {
        Task<ServiceResult<Empty>> CreateAsync(int projectId, string? userId, SprintDto dto);
        Task<ServiceResult<Empty>> DeleteAsync(int sprintId);
        Task<ServiceResult<Empty>> MoveBacklogTaskToSprintAsync(int workId, int sprintId);
        Task<ServiceResult<Empty>> UpdateTaskSprintAsync(string? userId, UpdateSprintDto dto);
    }
}
