using Microsoft.AspNetCore.Mvc;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Comments;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentQueryService _commentQueryService;
        private readonly ICommentCommandService _commentCommandService;

        public CommentController(ICommentQueryService commentQueryService, ICommentCommandService commentCommandService)
        {
            _commentQueryService = commentQueryService;
            _commentCommandService = commentCommandService;
        }

        // GET: api/<CommentController>
        [HttpGet("Commits")]
        public async Task<IActionResult> Get()
        {
            return this.ToActionResult(await _commentQueryService.GetAllAsync());
        }

        [HttpGet("CommitCount")]
        public async Task<IActionResult> GetCommintCount()
        {
            return this.ToActionResult(await _commentQueryService.GetCountAsync());
        }

        // GET api/<CommentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return this.ToActionResult(await _commentQueryService.GetByIdAsync(id));
        }

        // POST api/<CommentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CommentDto value)
        {
            return this.ToActionResult(await _commentCommandService.AddAsync(value));
        }

        // PUT api/<CommentController>/5
        [HttpPut("ChangeContext/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] string value)
        {
            return this.ToActionResult(await _commentCommandService.ChangeContextAsync(id, value));
        }

        // DELETE api/<CommentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return this.ToActionResult(await _commentCommandService.DeleteAsync(id));
        }
    }
}
