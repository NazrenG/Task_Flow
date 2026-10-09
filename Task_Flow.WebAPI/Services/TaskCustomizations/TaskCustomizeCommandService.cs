using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.TaskCustomizations
{
    public class TaskCustomizeCommandService : ITaskCustomizeCommandService
    {
        private readonly ITaskCustomizeService _taskCustomizeService;

        public TaskCustomizeCommandService(ITaskCustomizeService taskCustomizeService)
        {
            _taskCustomizeService = taskCustomizeService;
        }

        public async Task<ServiceResult<TaskCustomize>> AddAsync(TaskCustomizeDto value)
        {
            var item = value.ToTaskCustomize();
            await _taskCustomizeService.Add(item);

            return ServiceResult<TaskCustomize>.Success(item);
        }

        public Task<ServiceResult<Empty>> ChangeBackColorAsync(int id, string color)
        {
            return UpdateAsync(id, item => item.BackColor = color);
        }

        public Task<ServiceResult<Empty>> ChangeTagColorAsync(int id, string color)
        {
            return UpdateAsync(id, item => item.TagColor = color);
        }

        public async Task<ServiceResult<Empty>> DeleteAsync(int id)
        {
            var item = await _taskCustomizeService.GetCustomizeById(id);
            if (item == null)
            {
                return ServiceResult<Empty>.NotFound();
            }

            await _taskCustomizeService.Delete(item);
            return ServiceResult<Empty>.Success(Empty.Value);
        }

        private async Task<ServiceResult<Empty>> UpdateAsync(int id, Action<TaskCustomize> applyChange)
        {
            var item = await _taskCustomizeService.GetCustomizeById(id);
            if (item == null)
            {
                return ServiceResult<Empty>.NotFound();
            }

            applyChange(item);
            await _taskCustomizeService.Update(item);

            return ServiceResult<Empty>.Success(Empty.Value);
        }
    }
}
