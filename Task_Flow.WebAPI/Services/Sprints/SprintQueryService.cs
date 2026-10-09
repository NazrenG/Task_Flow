using Task_Flow.Business.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Sprints
{
    public class SprintQueryService : ISprintQueryService
    {
        private readonly ISprintService _sprintService;

        public SprintQueryService(ISprintService sprintService)
        {
            _sprintService = sprintService;
        }

        // Yalnız id və ad (seçim siyahıları üçün)
        public async Task<ServiceResult<List<object>>> GetSprintSummariesAsync(int projectId)
        {
            var sprints = await _sprintService.GetSprints(projectId);
            var summaries = sprints.Select(s => (object)new { s.Id, s.Name }).ToList();

            return ServiceResult<List<object>>.Success(summaries);
        }

        public Task<List<Sprint>> GetSprintsAsync(int projectId)
        {
            return _sprintService.GetSprints(projectId);
        }
    }
}
