using Microsoft.AspNetCore.Mvc;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.TaskCustomizations;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskCustomizeController : ControllerBase
    {
        private readonly ITaskCustomizeQueryService _queryService;
        private readonly ITaskCustomizeCommandService _commandService;

        public TaskCustomizeController(ITaskCustomizeQueryService queryService, ITaskCustomizeCommandService commandService)
        {
            _queryService = queryService;
            _commandService = commandService;
        }

        // GET: api/<TaskCustomizeController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return this.ToActionResult(await _queryService.GetAllAsync());
        }

        [HttpGet("BackColors")]
        public async Task<IActionResult> GetBackColors()
        {
            return this.ToActionResult(await _queryService.GetBackColorsAsync());
        }

        [HttpGet("TagColors")]
        public async Task<IActionResult> GetTagColors()
        {
            return this.ToActionResult(await _queryService.GetTagColorsAsync());
        }

        // GET api/<TaskCustomizeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return this.ToActionResult(await _queryService.GetByIdAsync(id));
        }

        // POST api/<TaskCustomizeController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TaskCustomizeDto value)
        {
            return this.ToActionResult(await _commandService.AddAsync(value));
        }

        // PUT api/<TaskCustomizeController>/5
        [HttpPut("BackgroundColor/{id}")]
        public async Task<IActionResult> PutBackColor(int id, [FromBody] string value)
        {
            return this.ToActionResult(await _commandService.ChangeBackColorAsync(id, value));
        }

        [HttpPut("TagColor/{id}")]
        public async Task<IActionResult> PutTagColor(int id, [FromBody] string value)
        {
            return this.ToActionResult(await _commandService.ChangeTagColorAsync(id, value));
        }

        // DELETE api/<TaskCustomizeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return this.ToActionResult(await _commandService.DeleteAsync(id));
        }
    }
}
