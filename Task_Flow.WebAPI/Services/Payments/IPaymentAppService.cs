using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Payments
{
    /// <summary>
    /// Ödəniş provayderi (Stripe) ilə ödəniş niyyəti (payment intent) yaradır.
    /// </summary>
    public interface IPaymentAppService
    {
        // amount: ən kiçik valyuta vahidi ilə (sent)
        Task<ServiceResult<object>> CreatePaymentIntentAsync(long amount);
    }
}
