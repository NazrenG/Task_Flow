using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.ProjectActivities;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectActivityController : ControllerBase
    {
        private readonly IProjectActivityAppService _projectActivityService;

        public ProjectActivityController(IProjectActivityAppService projectActivityService)
        {
            _projectActivityService = projectActivityService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [Authorize]
        [HttpGet("TeamMemberActivities")]
        public async Task<IActionResult> GetTeamActivities()
        {
            return this.ToActionResult(await _projectActivityService.GetOwnedProjectsActivitiesAsync(CurrentUserId));
        }

        [Authorize]
        [HttpGet("TeamMemberActivities/{projectId}")]
        public async Task<IActionResult> GetTeamActivitiesForProjectId(int projectId)
        {
            return this.ToActionResult(await _projectActivityService.GetProjectActivitiesAsync(projectId, CurrentUserId));
        }

        [Authorize]
        [HttpPost("AddTeamMemberActivities")]
        public async Task<IActionResult> AddTeamActivities([FromBody] ProjectActivityDto dto)
        {
            return this.ToActionResult(await _projectActivityService.AddAsync(CurrentUserId, dto));
        }
    }
}
