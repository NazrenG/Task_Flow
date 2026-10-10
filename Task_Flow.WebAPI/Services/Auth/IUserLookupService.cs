using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Auth
{
    /// <summary>
    /// İstifadəçi axtarışı və istifadəçi məlumatlarının oxunması.
    /// </summary>
    public interface IUserLookupService
    {
        Task<ServiceResult<object>> SearchUsersAsync(string? currentUserId, string key);
        Task<ServiceResult<List<CustomUser>>> GetUsersByNameAsync(string username);
        Task<ServiceResult<object>> GetCurrentUserAsync(string userId);
        Task<ServiceResult<int>> GetUserCountAsync();
        Task<ServiceResult<CustomUser>> GetUserByEmailAsync(string email);
    }
}
