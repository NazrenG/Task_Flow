using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.TaskCustomizations
{
    public class TaskCustomizeQueryService : ITaskCustomizeQueryService
    {
        private readonly ITaskCustomizeService _taskCustomizeService;

        public TaskCustomizeQueryService(ITaskCustomizeService taskCustomizeService)
        {
            _taskCustomizeService = taskCustomizeService;
        }

        public async Task<ServiceResult<List<TaskCustomizeDto>>> GetAllAsync()
        {
            var items = await _taskCustomizeService.GetCustomize();
            if (items == null)
            {
                return ServiceResult<List<TaskCustomizeDto>>.NotFound();
            }

            return ServiceResult<List<TaskCustomizeDto>>.Success(items.Select(i => i.ToTaskCustomizeDto()).ToList());
        }

        public async Task<ServiceResult<TaskCustomizeDto>> GetByIdAsync(int id)
        {
            var item = await _taskCustomizeService.GetCustomizeById(id);
            if (item == null)
            {
                return ServiceResult<TaskCustomizeDto>.NotFound();
            }

            return ServiceResult<TaskCustomizeDto>.Success(item.ToTaskCustomizeDto());
        }

        public Task<ServiceResult<List<string?>>> GetBackColorsAsync()
        {
            return GetDistinctColorsAsync(c => c.BackColor);
        }

        public Task<ServiceResult<List<string?>>> GetTagColorsAsync()
        {
            return GetDistinctColorsAsync(c => c.TagColor);
        }

        // İstifadə olunmuş rənglərin təkrarsız siyahısı
        private async Task<ServiceResult<List<string?>>> GetDistinctColorsAsync(Func<TaskCustomize, string?> colorSelector)
        {
            var items = await _taskCustomizeService.GetCustomize();
            if (items == null)
            {
                return ServiceResult<List<string?>>.NotFound();
            }

            return ServiceResult<List<string?>>.Success(items.Select(colorSelector).Distinct().ToList());
        }
    }
}
