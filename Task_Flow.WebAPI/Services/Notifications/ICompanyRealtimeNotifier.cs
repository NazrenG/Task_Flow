using Task_Flow.Entities.Enums;

namespace Task_Flow.WebAPI.Services.Notifications
{
    /// <summary>
    /// Şirkət işçilərinə plan dəyişikliyini SignalR ilə bildirir.
    /// </summary>
    public interface ICompanyRealtimeNotifier
    {
        Task NotifyPlanChangedAsync(IReadOnlyList<string> userIds, PlanType newPlan);
    }
}
