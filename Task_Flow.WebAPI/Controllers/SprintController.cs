using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.DTOs;
using Task_Flow.Entities.Models;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SprintController : ControllerBase
    {
        private readonly ISprintService spritService;

        public SprintController(ISprintService spritService)
        {
            this.spritService = spritService;
        }

        [Authorize]
        [HttpPost("NewSprint/{projectId}")]
        public async Task<IActionResult> CreateSprit(int projectId, [FromBody] SprintDto splitDto)
        {
            await spritService.Create(projectId, splitDto);
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
            await spritService.UpdateTaskSplit(updateSprintDto);
            return Ok();
        }

    }
}
