using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.Business.DTOs;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SprintController : ControllerBase
    {
        private readonly ISprintService spritService;
        private readonly ICanbanColumnService canbanColumnService;
        private readonly IHubContext<ConnectionHub> _context;

        public SprintController(ISprintService spritService, IHubContext<ConnectionHub> context, ICanbanColumnService canbanColumnService)
        {
            this.spritService = spritService;
            _context = context;
            this.canbanColumnService = canbanColumnService;
        }

        [HttpGet("ProjectSprints/{projectId}")]
        public async Task<IActionResult> GetProjectSprints(int projectId)
        {
            var sprints = await spritService.GetSprints(projectId);
            return Ok(sprints.Select(s => new { s.Id, s.Name }));
        }
        private async Task CreateDefaultKanbanColumns(int sprintId)
        {
            var columns = new[]
            {
        new { Name = "To Do", StatusKey = "to do" },
        new { Name = "Progress", StatusKey = "progress" },
        new { Name = "Done", StatusKey = "done" }
    };

            int order = 1;
            foreach (var col in columns)
            {
                await canbanColumnService.CreateDefaultCanbanName(
                    new CreateDefaultCanbanNameDto
                    {
                        SprintId = sprintId,
                        Name = col.Name,
                        Order = order++,
                        StatusKey = col.StatusKey
                    });
            }
        }

        [Authorize]
        [HttpPost("NewSprint/{projectId}")]
        public async Task<IActionResult> CreateSprit(int projectId, [FromBody] SprintDto splitDto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

          var sprint=  await spritService.Create(projectId, splitDto);
            //backlog
            await _context.Clients.User(userId).SendAsync("UpdateSprints");

            //default cNBn nAME
            await CreateDefaultKanbanColumns(sprint.Id);
            return Ok();

        }

        [Authorize]
        [HttpDelete("DeleteSprint/{spritId}")]
        public async Task<IActionResult> DeleteSprit(int spritId)
        {
            await spritService.DeleteSplit(spritId);
            return Ok();
        }

        [Authorize]
        [HttpPut("UpdatedSprint/{workId}/{spritId}")]
        public async Task<IActionResult> UpdatedSprit(int workId, int spritId)
        {
            await spritService.AddBacklogToSplit(workId, spritId);
            return Ok();
        }
        [Authorize]
        [HttpGet("AllSprints/{projectId}")]
        public async Task<List<Sprint>> GetSprints(int projectId)
        {

            var list = await spritService.GetSprints(projectId);

        

            return list;

        }

        [Authorize]
        [HttpPut("UpdateTaskSprint")]

        public async Task<IActionResult> UpdateSprint(UpdateSprintDto updateSprintDto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await spritService.UpdateTaskSplit(updateSprintDto);
           await _context.Clients.User(userId).SendAsync("AddProjectToSprint");
            return Ok();
        }

    }
}
