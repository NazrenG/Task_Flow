using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Sprints
{
    /// <summary>
    /// Layihənin sprint-lərini oxuyan əməliyyatlar.
    /// </summary>
    public interface ISprintQueryService
    {
        Task<ServiceResult<List<object>>> GetSprintSummariesAsync(int projectId);
        Task<List<Sprint>> GetSprintsAsync(int projectId);
    }
}
