using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Friends;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IFriendQueryService _friendQueryService;
        private readonly IFriendCommandService _friendCommandService;

        public FriendController(IFriendQueryService friendQueryService, IFriendCommandService friendCommandService)
        {
            _friendQueryService = friendQueryService;
            _friendCommandService = friendCommandService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        private IActionResult UserNotAuthenticated() => BadRequest(new { message = "User not authenticated." });

        [Authorize]
        [HttpGet("AllUser")]
        public async Task<IActionResult> GetAllUsers()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _friendQueryService.GetAllUsersAsync(userId));
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            try
            {
                return this.ToActionResult(await _friendCommandService.DeleteRequestAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/<FriendController>
        [Authorize]
        [HttpGet("AllFriends")]
        public async Task<IActionResult> GetAllFriends()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _friendQueryService.GetFriendsAsync(userId));
        }

        [Authorize]
        [HttpGet("AllFriendsForGroupChat")]
        public async Task<IActionResult> GetFriends()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _friendQueryService.GetFriendsForGroupChatAsync(userId));
        }

        // POST api/<FriendController>
        [Authorize]
        [HttpPost("NewFriend")]
        public async Task<IActionResult> Post([FromBody] SendFollowFriendDto value)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _friendCommandService.FollowAsync(userId, value));
        }

        [Authorize]
        [HttpDelete("UnFollow/{friendMail}")]
        public async Task<IActionResult> DeleteFriend(string friendMail)
        {
            var userId = CurrentUserId;
            if (userId == null) return Unauthorized(new { message = "user not found" });

            return this.ToActionResult(await _friendCommandService.UnfollowAsync(userId, friendMail));
        }
    }
}
