using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Projects
{
    /// <summary>
    /// Layihənin state-ini dəyişən əməliyyatlar.
    /// </summary>
    public interface IProjectCommandService
    {
        Task<ServiceResult<object>> CreateProjectAsync(string? userId, CreateProjectDto value);
        Task<ServiceResult<Empty>> UpdateProjectAsync(int id, string? userId, PutProjectDto dto);
        Task<ServiceResult<Empty>> ChangeTitleAsync(int id, string? userId, string value);
        Task<ServiceResult<Empty>> ChangeDescriptionAsync(int id, string? userId, string value);
        Task<ServiceResult<Empty>> ChangeCompletedAsync(int id, string? userId, bool value);
        Task<ServiceResult<Project>> DeleteProjectAsync(int id, string? userId);
        Task<ServiceResult<string>> UpdateTaskColumnAsync(string? userId, UpdateTaskColumnDto dto);
    }
}
