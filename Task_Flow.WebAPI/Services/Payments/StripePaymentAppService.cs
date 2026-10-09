using Stripe;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Payments
{
    public class StripePaymentAppService : IPaymentAppService
    {
        private const string Currency = "usd";

        public async Task<ServiceResult<object>> CreatePaymentIntentAsync(long amount)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = Currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                }
            };

            // Stripe API açarı Program.cs-də StripeConfiguration.ApiKey ilə təyin olunur
            var paymentIntent = await new PaymentIntentService().CreateAsync(options);

            return ServiceResult<object>.Success(new { clientSecret = paymentIntent.ClientSecret });
        }
    }
}
