using Task_Flow.Business.Abstract;
using Task_Flow.Entities.Enums;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Companies
{
    public class CompanyCommandService : ICompanyCommandService
    {
        private readonly ICompanyService _companyService;
        private readonly ICompanyWorkerService _companyWorkerService;
        private readonly ICompanyRealtimeNotifier _notifier;

        public CompanyCommandService(
            ICompanyService companyService,
            ICompanyWorkerService companyWorkerService,
            ICompanyRealtimeNotifier notifier)
        {
            _companyService = companyService;
            _companyWorkerService = companyWorkerService;
            _notifier = notifier;
        }

        public async Task<ServiceResult<Empty>> CreateAsync(string? ownerId, CreateCompanyDto value)
        {
            await _companyService.CreateCompanyAsync(new Company
            {
                Address = value.Address,
                OwnerId = ownerId!,
                CreatedDate = DateTime.UtcNow,
                Email = value.Email,
                Name = value.Name
            });

            return Ok();
        }

        public async Task<ServiceResult<Empty>> UpdateAsync(int id, CreateCompanyDto value)
        {
            await _companyService.UpdateCompany(id, value.Name, value.Email, value.Address);
            return Ok();
        }

        // Ödəniş edildikdə şirkət yenidən aktivləşir və işçilər yenidən şirkət planına keçir
        public async Task<ServiceResult<Empty>> MarkAsPaidAsync(string? ownerId)
        {
            await _companyService.UserPaidForCompany(ownerId!);

            var company = await _companyService.GetCompany(ownerId!);
            var workerIds = await _companyWorkerService.CompanyReopened(company.CompanyId);
            await _notifier.NotifyPlanChangedAsync(workerIds, PlanType.CompanyWorker);

            return Ok();
        }

        // Şirkət silinəndə işçilər pulsuz plana keçir
        public async Task<ServiceResult<Empty>> DeleteAsync(int id)
        {
            await _companyService.DeleteCompany(id);

            var workerIds = await _companyWorkerService.CompanyDeleted(id);
            await _notifier.NotifyPlanChangedAsync(workerIds, PlanType.Free);

            return Ok();
        }

        public async Task<ServiceResult<Empty>> RemoveWorkerAsync(int workerId)
        {
            await _companyWorkerService.RemoveCompanyWorker(workerId);
            return Ok();
        }

        private static ServiceResult<Empty> Ok() => ServiceResult<Empty>.Success(Empty.Value);
    }
}
