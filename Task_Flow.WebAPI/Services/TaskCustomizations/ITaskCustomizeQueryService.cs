using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.TaskCustomizations
{
    /// <summary>
    /// Task rəng ayarlarını oxuyan əməliyyatlar.
    /// </summary>
    public interface ITaskCustomizeQueryService
    {
        Task<ServiceResult<List<TaskCustomizeDto>>> GetAllAsync();
        Task<ServiceResult<TaskCustomizeDto>> GetByIdAsync(int id);
        Task<ServiceResult<List<string?>>> GetBackColorsAsync();
        Task<ServiceResult<List<string?>>> GetTagColorsAsync();
    }
}
