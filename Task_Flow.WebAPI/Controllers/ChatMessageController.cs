using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Chats;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatMessageController : ControllerBase
    {
        private const string InvalidTokenMessage = "Invalid token or user not found.";

        private readonly IChatMessageQueryService _chatMessageQueryService;
        private readonly IChatMessageCommandService _chatMessageCommandService;

        public ChatMessageController(IChatMessageQueryService chatMessageQueryService, IChatMessageCommandService chatMessageCommandService)
        {
            _chatMessageQueryService = chatMessageQueryService;
            _chatMessageCommandService = chatMessageCommandService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [Authorize]
        [HttpPost("NewMessage")]
        public async Task<IActionResult> Post([FromBody] ChatMessageDto dto)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId)) return Unauthorized(InvalidTokenMessage);

            return this.ToActionResult(await _chatMessageCommandService.SendMessageAsync(userId, dto));
        }

        [Authorize]
        [HttpGet("AllMessages/{friendMail}")]
        public async Task<IActionResult> Get(string friendMail)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId)) return Unauthorized(InvalidTokenMessage);

            return this.ToActionResult(await _chatMessageQueryService.GetConversationAsync(userId, friendMail));
        }

        [HttpDelete("RemoveMessage/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return this.ToActionResult(await _chatMessageCommandService.DeleteMessageAsync(id));
        }
    }
}
