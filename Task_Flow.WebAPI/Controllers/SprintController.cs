using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
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
        private readonly IHubContext<ConnectionHub> _context;

        public SprintController(ISprintService spritService, IHubContext<ConnectionHub> context)
        {
            this.spritService = spritService;
            _context = context;
        }

        [Authorize]
        [HttpPost("NewSprint/{projectId}")]
        public async Task<IActionResult> CreateSprit(int projectId, [FromBody] SprintDto splitDto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await spritService.Create(projectId, splitDto);
            //backlog
            await _context.Clients.User(userId).SendAsync("UpdateSprints");
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
