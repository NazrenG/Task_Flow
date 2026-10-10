using Task_Flow.Business.Abstract;
using Task_Flow.Business.DTOs;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Sprints
{
    public class SprintCommandService : ISprintCommandService
    {
        // Hər yeni sprint üçün yaradılan Kanban sütunları (sıra ilə)
        private static readonly (string Name, string StatusKey)[] DefaultKanbanColumns =
        {
            ("To Do", "to do"),
            ("Progress", "progress"),
            ("Done", "done")
        };

        private readonly ISprintService _sprintService;
        private readonly ICanbanColumnService _canbanColumnService;
        private readonly ISprintRealtimeNotifier _notifier;

        public SprintCommandService(
            ISprintService sprintService,
            ICanbanColumnService canbanColumnService,
            ISprintRealtimeNotifier notifier)
        {
            _sprintService = sprintService;
            _canbanColumnService = canbanColumnService;
            _notifier = notifier;
        }

        public async Task<ServiceResult<Empty>> CreateAsync(int projectId, string? userId, SprintDto dto)
        {
            var sprint = await _sprintService.Create(projectId, dto);
            await _notifier.NotifySprintsChangedAsync(userId!);
            await CreateDefaultKanbanColumnsAsync(sprint.Id);

            return Ok();
        }

        public async Task<ServiceResult<Empty>> DeleteAsync(int sprintId)
        {
            await _sprintService.DeleteSplit(sprintId);
            return Ok();
        }

        public async Task<ServiceResult<Empty>> MoveBacklogTaskToSprintAsync(int workId, int sprintId)
        {
            await _sprintService.AddBacklogToSplit(workId, sprintId);
            return Ok();
        }

        public async Task<ServiceResult<Empty>> UpdateTaskSprintAsync(string? userId, UpdateSprintDto dto)
        {
            await _sprintService.UpdateTaskSplit(dto);
            await _notifier.NotifyTaskSprintChangedAsync(userId!);
            return Ok();
        }

        private async Task CreateDefaultKanbanColumnsAsync(int sprintId)
        {
            var order = 1;
            foreach (var (name, statusKey) in DefaultKanbanColumns)
            {
                await _canbanColumnService.CreateDefaultCanbanName(new CreateDefaultCanbanNameDto
                {
                    SprintId = sprintId,
                    Name = name,
                    Order = order++,
                    StatusKey = statusKey
                });
            }
        }

        private static ServiceResult<Empty> Ok() => ServiceResult<Empty>.Success(Empty.Value);
    }
}
