using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Services.Mapping
{
    public static class UserTaskMappingExtensions
    {
        private const string InitialStatus = "to do";

        public static WorkDto ToWorkDto(this UserTask task, string userId)
        {
            return new WorkDto
            {
                Id = task.Id,
                CreatedById = userId,
                Description = task.Description,
                Deadline = task.Deadline,
                Priority = task.Priority,
                Status = task.Status,
                Title = task.Title,
                StartDate = task.StartTime,
                Color = task.Color
            };
        }

        // Tək task üçün: Id göndərilmir
        public static WorkDto ToSingleWorkDto(this UserTask task, string userId)
        {
            var dto = task.ToWorkDto(userId);
            dto.Id = default;
            return dto;
        }

        public static UserTask ToNewUserTask(this WorkDto dto, string userId)
        {
            return new UserTask
            {
                CreatedById = userId,
                Description = dto.Description,
                Deadline = dto.Deadline,
                Priority = dto.Priority,
                Status = InitialStatus,
                Title = dto.Title,
                Color = dto.Color,
                StartTime = dto.StartDate
            };
        }

        public static void ApplyEdit(this UserTask task, WorkDto dto)
        {
            task.Description = dto.Description;
            task.Deadline = dto.Deadline;
            task.Priority = dto.Priority;
            task.Status = dto.Status;
            task.Title = dto.Title;
            task.StartTime = dto.StartDate;
            task.Color = dto.Color;
        }

        public static void ApplyCalendarEdit(this UserTask task, EditForCalendarDto dto)
        {
            task.Deadline = dto.Deadline;
            task.StartTime = dto.StartDate;
        }
    }
}
