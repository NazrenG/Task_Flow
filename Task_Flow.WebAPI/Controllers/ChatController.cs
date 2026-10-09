using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Services.Chats;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatQueryService _chatQueryService;

        public ChatController(IChatQueryService chatQueryService)
        {
            _chatQueryService = chatQueryService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        private IActionResult UserNotAuthenticated() => BadRequest(new { message = "User not authenticated." });

        [Authorize]
        [HttpGet("AllChatsWithFriends")]
        public async Task<IActionResult> GetAllFriends()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _chatQueryService.GetChatListAsync(userId));
        }

        [Authorize]
        [HttpGet("UserMessages")]
        public async Task<IActionResult> GetUserMessages()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _chatQueryService.GetIncomingMessagesAsync(userId));
        }
    }
}
