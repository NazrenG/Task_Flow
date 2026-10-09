using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Enums;
using Task_Flow.Entities.Models;

namespace Task_Flow.WebAPI.Services.NotificationCenter.RequestAcceptance
{
    // Şirkət dəvəti qəbul edildikdə istifadəçi şirkətin işçisi olur
    public class CompanyWorkerRequestAcceptHandler : IRequestAcceptHandler
    {
        private const int DefaultWorkerRole = 1;

        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyWorkerService _companyWorkerService;

        public CompanyWorkerRequestAcceptHandler(
            IUserService userService,
            ICompanyService companyService,
            ICompanyWorkerService companyWorkerService)
        {
            _userService = userService;
            _companyService = companyService;
            _companyWorkerService = companyWorkerService;
        }

        public string NotificationType => RequestNotificationTypes.CompanyWorkerRequest;

        public async Task HandleAsync(RequestNotification request, string userId)
        {
            var currentUser = await _userService.GetUserById(userId);
            var company = await _companyService.GetCompany(request.SenderId!);

            await _companyWorkerService.AddWorkerToCompany(new CompanyWorker
            {
                Occupation = currentUser.Occupation!,
                CompanyId = company.CompanyId,
                Role = DefaultWorkerRole,
                UserId = userId
            });

            // Diqqət: bu dəyişiklik ayrıca saxlanılmır, eyni DbContext-də növbəti
            // SaveChanges (recent activity log) zamanı yazılır.
            currentUser.PlanType = PlanType.CompanyWorker;
        }
    }
}
