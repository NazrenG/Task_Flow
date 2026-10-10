using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.GroupChats;

namespace Task_Flow.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupChatController : ControllerBase
    {
        private readonly IGroupChatQueryService _groupChatQueryService;
        private readonly IGroupChatCommandService _groupChatCommandService;

        public GroupChatController(IGroupChatQueryService groupChatQueryService, IGroupChatCommandService groupChatCommandService)
        {
            _groupChatQueryService = groupChatQueryService;
            _groupChatCommandService = groupChatCommandService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet("GetChatAdmin/{groupId}")]
        public async Task<IActionResult> GetAdmin(int groupId)
        {
            if (groupId == 0) return BadRequest();

            return this.ToActionResult(await _groupChatQueryService.GetAdminAsync(groupId));
        }

        [HttpGet("GetUserGroupChats")]
        public async Task<IActionResult> Get()
        {
            return this.ToActionResult(await _groupChatQueryService.GetUserGroupChatsAsync(CurrentUserId));
        }

        [HttpPost("CreateChatGroup")]
        public async Task<IActionResult> Post([FromBody] CreateChatGroupDto value)
        {
            return this.ToActionResult(await _groupChatCommandService.CreateGroupAsync(CurrentUserId, value));
        }

        [HttpPost("SendMessageToGroup")]
        public async Task<IActionResult> SendMessageToGroup([FromBody] SendMessageToGroupDto value)
        {
            return this.ToActionResult(await _groupChatCommandService.SendMessageAsync(CurrentUserId, value));
        }

        [HttpPost("AddMembersToGroup")]
        public async Task<IActionResult> AddMemberToGroup([FromBody] AddNewMembersGroupChatDto dto)
        {
            return this.ToActionResult(await _groupChatCommandService.AddMembersAsync(dto));
        }

        [HttpGet("GetAllGroupMessages/{id}")]
        public async Task<IActionResult> GetGroupMessages(int id)
        {
            if (id == 0) return BadRequest();

            return this.ToActionResult(await _groupChatQueryService.GetMessagesAsync(id, CurrentUserId));
        }

        [HttpGet("GetGroupChatDetails/{id}")]
        public async Task<IActionResult> GetGroupChatDetails(int id)
        {
            if (id == 0) return BadRequest();

            return this.ToActionResult(await _groupChatQueryService.GetDetailsAsync(id, CurrentUserId));
        }

        [HttpGet("SearchFriendsForChat/{id}")]
        public async Task<IActionResult> SearchFriendsForChat(int id, [FromQuery] string key)
        {
            return this.ToActionResult(await _groupChatQueryService.SearchFriendsForChatAsync(id, CurrentUserId, key));
        }

        [HttpPut("UpdateGroupName/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] string value)
        {
            return this.ToActionResult(await _groupChatCommandService.RenameGroupAsync(id, value));
        }

        [HttpDelete("RemoveGroupMember/{id}")]
        public async Task<IActionResult> RemoveGroupMember(int id, [FromBody] string userId)
        {
            return this.ToActionResult(await _groupChatCommandService.RemoveMemberAsync(id, userId, CurrentUserId));
        }

        [HttpDelete("DeleteGroup/{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            return this.ToActionResult(await _groupChatCommandService.DeleteGroupAsync(id, CurrentUserId));
        }

        [HttpPut("ExitGroup/{groupId}")]
        public async Task<IActionResult> ExitGroup(int groupId)
        {
            return this.ToActionResult(await _groupChatCommandService.ExitGroupAsync(groupId, CurrentUserId));
        }
    }
}
