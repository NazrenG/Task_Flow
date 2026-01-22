using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly IProjectService _projectService;
        private readonly ITeamMemberService _memberService;
        private IUserService _userService;
        private readonly static List<string> OccupationList = new List<string>
        {
             "IT (Programming, Systems)",
          "Design (Graphic, UI/UX)",
          "Human Resources",
          "Software Programming",
          "Backend Developer",
          "Frontend Developer",
          "Other (please specify)"
        };
        private readonly static List<string> ProfessionList = new List<string>
        {
            "Programming",
          "Marketing",
          "Accounting",
          "Education",
          "Other (please specify)"
        };
        public QuizController(IQuizService quizService, IProjectService projectService, ITeamMemberService memberService, IUserService userService)
        {
            _quizService = quizService;
            _projectService = projectService;
            _userService = userService;
            _memberService = memberService;
        }

        //occupation statistic
        [HttpGet("OccupationStatistic")]
        public async Task<IActionResult> GetOccupationStatistic()
        {
            var list = await _quizService.Quizzes();
            var totalCount = list.Count;

            var occupationStatistics = new List<OccupationStatisticDto>();

            foreach (var item in OccupationList)
            {
                var count = await _quizService.SpecialOccupationCount(item);
                var percentage = totalCount > 0 ? Math.Round(count * 100m / totalCount, 2) : 0;
                occupationStatistics.Add(new OccupationStatisticDto
                {
                    OccupationName = item,
                    Percentage = percentage
                });
            }

            return Ok(occupationStatistics);
        }

        //profession statistic
        [HttpGet("ProfessionStatistic")]
        public async Task<IActionResult> GetProfessionStatistic()
        {
            var list = await _quizService.Quizzes();
            var totalCount = list.Count;

            var professionStatistics = new List<OccupationStatisticDto>();

            foreach (var item in OccupationList)
            {
                var count = await _quizService.SpecialOccupationCount(item);
                var percentage = totalCount > 0 ? Math.Round(count * 100m / totalCount, 2) : 0;
                professionStatistics.Add(new OccupationStatisticDto
                {
                    OccupationName = item,
                    Percentage = percentage
                });
            }

            return Ok(professionStatistics);
        }

        [Authorize]
        [HttpGet("OccupationStatisticInProjects")]
        public async Task<IActionResult> GetOccupationStatisticInProjects()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            var userProjects = await _projectService.GetProjects(userId);
            if (userProjects == null || !userProjects.Any())
            {

                var emptyStatistics = OccupationList.Select(occupation => new OccupationStatisticDto
                {
                    OccupationName = occupation,
                    Percentage = 0
                }).ToList();
                return Ok(emptyStatistics);
            }

            var projectIds = userProjects.Select(p => p.Id).ToList();


            var members = await _memberService.GetUsersByProjectIdsAsync(projectIds);


            if (members == null || !members.Any())
            {
                var emptyStatistics = OccupationList.Select(occupation => new OccupationStatisticDto
                {
                    OccupationName = occupation,
                    Percentage = 0
                }).ToList();
                return Ok(emptyStatistics);
            }

            var totalCount = members.Count;
            var occupationStatistics = OccupationList.Select(occupation =>
            {
                var count = members.Count(m => m.Occupation == occupation);
                var percentage = totalCount > 0 ? Math.Round(count * 100m / totalCount, 2) : 0;
                return new OccupationStatisticDto
                {
                    OccupationName = occupation,
                    Percentage = percentage
                };
            }).ToList();

            return Ok(occupationStatistics);
        }

        [HttpPost("NewQuiz")]
        public async Task<IActionResult> CreateQuiz([FromBody] QuizDto dto)
        {
            var quiz = new Quiz
            {
                Profession = dto.Profession,
                UsagePurpose = dto.UsagePurpose,
            };

            await _quizService.Add(quiz);

            return Ok();
        }

        //kohne

        //// put api/<QuizController>
        //[HttpPut("Profession")]
        //public async Task<IActionResult> PutProfession([FromBody] string value)
        //{
        //    var items = await _quizService.Quizzes();
        //    var last = items.Last();

        //    last.Profession = value;
        //    await _quizService.Update(last);
        //    return Ok();
        //}

        //[HttpPut("Occupation")]
        //public async Task<IActionResult> PutOccupation([FromBody] string value)
        //{
        //    var items = await _quizService.Quizzes();
        //    var last = items.Last();

        //    last.UsagePurpose = value;
        //    await _quizService.Update(last);
        //    return Ok();
        //}


    }
}