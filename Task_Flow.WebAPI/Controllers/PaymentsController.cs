using Microsoft.AspNetCore.Mvc;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Payments;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentAppService _paymentService;

        public PaymentsController(IPaymentAppService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentRequestDto request)
        {
            return this.ToActionResult(await _paymentService.CreatePaymentIntentAsync(request.Amount));
        }
    }
}
