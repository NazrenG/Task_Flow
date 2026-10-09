using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.Business.DTOs;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Services.Sprints;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SprintController : ControllerBase
    {
        private readonly ISprintQueryService _sprintQueryService;
        private readonly ISprintCommandService _sprintCommandService;

        public SprintController(ISprintQueryService sprintQueryService, ISprintCommandService sprintCommandService)
        {
            _sprintQueryService = sprintQueryService;
            _sprintCommandService = sprintCommandService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet("ProjectSprints/{projectId}")]
        public async Task<IActionResult> GetProjectSprints(int projectId)
        {
            return this.ToActionResult(await _sprintQueryService.GetSprintSummariesAsync(projectId));
        }

        [Authorize]
        [HttpPost("NewSprint/{projectId}")]
        public async Task<IActionResult> CreateSprit(int projectId, [FromBody] SprintDto splitDto)
        {
            return this.ToActionResult(await _sprintCommandService.CreateAsync(projectId, CurrentUserId, splitDto));
        }

        [Authorize]
        [HttpDelete("DeleteSprint/{spritId}")]
        public async Task<IActionResult> DeleteSprit(int spritId)
        {
            return this.ToActionResult(await _sprintCommandService.DeleteAsync(spritId));
        }

        [Authorize]
        [HttpPut("UpdatedSprint/{workId}/{spritId}")]
        public async Task<IActionResult> UpdatedSprit(int workId, int spritId)
        {
            return this.ToActionResult(await _sprintCommandService.MoveBacklogTaskToSprintAsync(workId, spritId));
        }

        [Authorize]
        [HttpGet("AllSprints/{projectId}")]
        public async Task<List<Sprint>> GetSprints(int projectId)
        {
            return await _sprintQueryService.GetSprintsAsync(projectId);
        }

        [Authorize]
        [HttpPut("UpdateTaskSprint")]
        public async Task<IActionResult> UpdateSprint(UpdateSprintDto updateSprintDto)
        {
            return this.ToActionResult(await _sprintCommandService.UpdateTaskSprintAsync(CurrentUserId, updateSprintDto));
        }
    }
}
