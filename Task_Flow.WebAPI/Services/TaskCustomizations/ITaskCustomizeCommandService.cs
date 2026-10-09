using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.TaskCustomizations
{
    /// <summary>
    /// Task rəng ayarlarını yaradan, dəyişən və silən əməliyyatlar.
    /// </summary>
    public interface ITaskCustomizeCommandService
    {
        Task<ServiceResult<TaskCustomize>> AddAsync(TaskCustomizeDto value);
        Task<ServiceResult<Empty>> ChangeBackColorAsync(int id, string color);
        Task<ServiceResult<Empty>> ChangeTagColorAsync(int id, string color);
        Task<ServiceResult<Empty>> DeleteAsync(int id);
    }
}
