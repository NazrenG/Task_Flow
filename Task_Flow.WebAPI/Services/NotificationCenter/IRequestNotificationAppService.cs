using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.NotificationCenter
{
    /// <summary>
    /// Sorğu bildirişləri: dostluq, layihə və şirkət dəvətləri.
    /// </summary>
    public interface IRequestNotificationAppService
    {
        Task<ServiceResult<List<object>>> GetPendingSummariesAsync(string userId);
        Task<ServiceResult<List<object>>> GetLatestPendingSummariesAsync(string userId);
        Task<ServiceResult<List<object>>> GetPendingRequestsAsync(string userId);
        Task<ServiceResult<int>> GetPendingCountAsync(string? userId);

        Task<ServiceResult<object>> SendRequestAsync(string userId, RequestNotificationDto dto);
        Task<ServiceResult<object>> SendCompanyWorkerRequestsAsync(string userId, CompanyRequestDto dto);
        Task<ServiceResult<object>> DeleteRequestAsync(int requestId, string userId);
        Task<ServiceResult<object>> AcceptRequestAsync(int requestId, string userId);
    }
}
