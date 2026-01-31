using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.DTOs;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CanbanController : ControllerBase
    {

        private readonly ICanbanColumnService canbanColumnService;

        public CanbanController(ICanbanColumnService canbanColumnService)
        {
            this.canbanColumnService = canbanColumnService;
        }

        [HttpPost("NewCanbanColumn")]
        public async Task<IActionResult> CreateCanbanColumn([FromBody] CreateCanbanDto canbanNameDto)
        {
            await canbanColumnService.CreateCanbanColumn(canbanNameDto);
            return Ok();
        }


        [HttpDelete("RemoveCanbanColumn/{id}")]
        public async Task<IActionResult> DeleteCanbanColumn(int id)
        {
            await canbanColumnService.DeleteCanbanColumn(id);
            return Ok();
        }

        [HttpGet("AllCanbanNames/{projectId}")]

        public async Task<IActionResult> AllCanbanNames(int projectId)
        {
            var list = await canbanColumnService.GetAllCanbanColumn(projectId);
            return Ok(list);
        }

    }
}
