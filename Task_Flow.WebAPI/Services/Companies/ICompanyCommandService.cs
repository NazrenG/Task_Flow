using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Companies
{
    /// <summary>
    /// Şirkəti yaradan, dəyişən, ödənişini qeyd edən və silən əməliyyatlar.
    /// </summary>
    public interface ICompanyCommandService
    {
        Task<ServiceResult<Empty>> CreateAsync(string? ownerId, CreateCompanyDto value);
        Task<ServiceResult<Empty>> UpdateAsync(int id, CreateCompanyDto value);
        Task<ServiceResult<Empty>> MarkAsPaidAsync(string? ownerId);
        Task<ServiceResult<Empty>> DeleteAsync(int id);
        Task<ServiceResult<Empty>> RemoveWorkerAsync(int workerId);
    }
}
