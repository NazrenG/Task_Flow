using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Companies
{
    /// <summary>
    /// Şirkət, işçilər və şirkət layihələrini oxuyan əməliyyatlar.
    /// </summary>
    public interface ICompanyQueryService
    {
        Task<ServiceResult<object>> GetOwnCompanyAsync(string userId);
        Task<ServiceResult<object>> GetSelectableUsersAsync(int companyId);
        Task<ServiceResult<object>> GetWorkersAsync(int companyId);
        Task<ServiceResult<object>> GetProjectsAsync(int companyId);
        Task<ServiceResult<object>> SearchWorkersAsync(string? userId, string key);
    }
}
