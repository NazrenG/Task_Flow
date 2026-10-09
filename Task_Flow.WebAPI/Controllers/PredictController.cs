using Microsoft.AspNetCore.Mvc;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Services.Predictions;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictController : ControllerBase
    {
        private readonly IWordCompletionService _wordCompletionService;

        public PredictController(IWordCompletionService wordCompletionService)
        {
            _wordCompletionService = wordCompletionService;
        }

        [HttpGet("complete")]
        public async Task<IActionResult> GetSuggestion([FromQuery] string prompt)
        {
            try
            {
                return this.ToActionResult(await _wordCompletionService.CompleteAsync(prompt));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
