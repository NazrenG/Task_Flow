using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Quizzes;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizAppService _quizService;
        private readonly IOccupationStatisticsService _statisticsService;

        public QuizController(IQuizAppService quizService, IOccupationStatisticsService statisticsService)
        {
            _quizService = quizService;
            _statisticsService = statisticsService;
        }

        // occupation statistic
        [HttpGet("OccupationStatistic")]
        public async Task<IActionResult> GetOccupationStatistic()
        {
            return this.ToActionResult(await _statisticsService.GetOverallStatisticsAsync());
        }

        // profession statistic (hazırda occupation statistikası ilə eyni nəticəni qaytarır)
        [HttpGet("ProfessionStatistic")]
        public async Task<IActionResult> GetProfessionStatistic()
        {
            return this.ToActionResult(await _statisticsService.GetOverallStatisticsAsync());
        }

        [Authorize]
        [HttpGet("OccupationStatisticInProjects")]
        public async Task<IActionResult> GetOccupationStatisticInProjects()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { message = "User not authenticated." });

            return this.ToActionResult(await _statisticsService.GetProjectMembersStatisticsAsync(userId));
        }

        [HttpPost("NewQuiz")]
        public async Task<IActionResult> CreateQuiz([FromBody] QuizDto dto)
        {
            return this.ToActionResult(await _quizService.CreateQuizAsync(dto));
        }
    }
}
