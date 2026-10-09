using Microsoft.AspNetCore.SignalR;
using Task_Flow.Entities.Enums;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Services.Notifications
{
    public class CompanyRealtimeNotifier : ICompanyRealtimeNotifier
    {
        private const string PlanDowngraded = "PlanDowngraded";

        private readonly IHubContext<ConnectionHub> _hubContext;

        public CompanyRealtimeNotifier(IHubContext<ConnectionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // Frontend planı rəqəm kimi gözləyir
        public Task NotifyPlanChangedAsync(IReadOnlyList<string> userIds, PlanType newPlan)
        {
            return _hubContext.Clients.Users(userIds).SendAsync(PlanDowngraded, (int)newPlan);
        }
    }
}
