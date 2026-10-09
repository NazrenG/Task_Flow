using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Services.Mapping
{
    public static class TaskCustomizeMappingExtensions
    {
        public static TaskCustomizeDto ToTaskCustomizeDto(this TaskCustomize customize)
        {
            return new TaskCustomizeDto
            {
                TagColor = customize.TagColor,
                BackColor = customize.BackColor,
                TaskId = customize.TaskId
            };
        }

        public static TaskCustomize ToTaskCustomize(this TaskCustomizeDto dto)
        {
            return new TaskCustomize
            {
                TagColor = dto.TagColor,
                BackColor = dto.BackColor,
                TaskId = dto.TaskId
            };
        }
    }
}
