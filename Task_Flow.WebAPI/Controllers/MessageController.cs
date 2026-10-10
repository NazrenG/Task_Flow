using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Chats;
using Task_Flow.WebAPI.Services.Messages;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageAppService _messageService;
        private readonly IChatQueryService _chatQueryService;

        public MessageController(IMessageAppService messageService, IChatQueryService chatQueryService)
        {
            _messageService = messageService;
            _chatQueryService = chatQueryService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        private IActionResult UserNotAuthenticated() => BadRequest("User not authenticated.");

        // GET: api/<MessageController>
        [Authorize]
        [HttpGet("UserMessage")]
        public async Task<IActionResult> Get()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _messageService.GetReceivedMessagesAsync(userId));
        }

        [Authorize]
        [HttpGet("TwoMessage")]
        public async Task<IActionResult> TakeTwoMessage()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _chatQueryService.GetLatestMessagesAsync(userId));
        }

        // userin bildirim sayi
        [Authorize]
        [HttpGet("UserMessageCount")]
        public async Task<IActionResult> GetCount()
        {
            return this.ToActionResult(await _chatQueryService.GetActiveChatCountAsync(CurrentUserId));
        }

        // POST api/<MessageController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MessageDto value)
        {
            return this.ToActionResult(await _messageService.AddAsync(value));
        }

        // DELETE api/<MessageController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return this.ToActionResult(await _messageService.DeleteAsync(id));
        }
    }
}
