using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Companies
{
    public class CompanyQueryService : ICompanyQueryService
    {
        private readonly ICompanyService _companyService;
        private readonly ICompanyWorkerService _companyWorkerService;
        private readonly IUserService _userService;

        public CompanyQueryService(
            ICompanyService companyService,
            ICompanyWorkerService companyWorkerService,
            IUserService userService)
        {
            _companyService = companyService;
            _companyWorkerService = companyWorkerService;
            _userService = userService;
        }

        // İstifadəçinin sahibi olduğu şirkət, sahib məlumatları ilə
        public async Task<ServiceResult<object>> GetOwnCompanyAsync(string userId)
        {
            var owner = await _userService.GetUserById(userId);
            var company = await _companyService.GetCompany(userId);
            if (company == null)
            {
                return ServiceResult<object>.Success(new { state = false });
            }

            company.OwnerEmail = owner.Email!;
            company.OwnerUsername = owner.UserName!;
            company.OwnerFullname = owner.Firstname + " " + owner.Lastname;

            return ServiceResult<object>.Success(new { Company = company, state = true });
        }

        public async Task<ServiceResult<object>> GetSelectableUsersAsync(int companyId)
        {
            var users = await _companyWorkerService.SelectedUsersForCompany(companyId);
            return ServiceResult<object>.Success(new { Users = users });
        }

        public async Task<ServiceResult<object>> GetWorkersAsync(int companyId)
        {
            var workers = await _companyWorkerService.GetAllCompanyWorkers(w => w.CompanyId == companyId);
            var result = new List<GetCompanyWorkersDto>();

            foreach (var worker in workers)
            {
                var user = await _userService.GetUserById(worker.UserId);
                result.Add(new GetCompanyWorkersDto
                {
                    Id = worker.Id,
                    Fullname = user.Firstname + " " + user.Lastname,
                    Username = user.UserName!,
                    Occupation = worker.Occupation
                });
            }

            return ServiceResult<object>.Success(new { Users = result });
        }

        public async Task<ServiceResult<object>> GetProjectsAsync(int companyId)
        {
            var projects = await _companyService.GetCompanyProjects(companyId);
            return ServiceResult<object>.Success(new { Users = projects });
        }

        // Axtarış istifadəçinin öz şirkətinin işçiləri arasında aparılır
        public async Task<ServiceResult<object>> SearchWorkersAsync(string? userId, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return ServiceResult<object>.BadRequest("Search key is required");
            }

            var company = await _companyService.GetCompany(userId!);
            var workers = await _companyWorkerService.SearchWorkerByKey(key, company.CompanyId);

            return ServiceResult<object>.Success(new { Users = workers });
        }
    }
}
