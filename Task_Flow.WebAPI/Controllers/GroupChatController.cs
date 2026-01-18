using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Enums;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Hubs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Task_Flow.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupChatController : ControllerBase
    {
        // GET: api/<GroupChatController>
        private readonly IGroupChatService _groupChatService;
        private readonly IUserService _userService;
        private readonly IFriendService _friendService;
        private readonly IHubContext<ConnectionHub> _context;
        public GroupChatController(IGroupChatService groupChatService,IUserService userService,IFriendService friendService,IHubContext<ConnectionHub> hubContext)
        {
            _groupChatService = groupChatService;
            _userService = userService;
            _friendService = friendService;
            _context = hubContext;
        }

        [HttpGet("GetChatAdmin/{groupId}")]
        public async Task<IActionResult> GetAdmin(int groupId)
        {
            if (groupId==0) { return BadRequest(); }
            var admin= await _groupChatService.GetGroupAdminAsync(groupId);
            if(admin==null) return NotFound();
            return Ok(new {Admin=admin});
        }

        // GET api/<GroupChatController>/5
        [HttpGet("GetUserGroupChats")]
        public async Task<IActionResult> Get()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var groupChatList=await _groupChatService.GetAllUserGroupChatsAsync(userId);
            return Ok(new { List =groupChatList});
        }

        // POST api/<GroupChatController>
        [HttpPost("CreateChatGroup")]
        public async Task<IActionResult> Post([FromBody] CreateChatGroupDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

           var groupId=await _groupChatService.CreateGroupChat(userId, value.GroupName);
            if (value.Members.Count() > 0) { await _groupChatService.AddMembersToGroupChat(groupId, value.Members); }
             //
            return Ok();
        }
        
        
        [HttpPost("SendMessageToGroup")]
        public async Task<IActionResult> SendMessageToGroup([FromBody] SendMessageToGroupDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var data=await _groupChatService.SendMessageToGroup(value.GroupId,userId,value.Message);
            var sender=await _userService.GetUserById(userId);
            var dto = new GroupChatMessageDto
            {
                Id = data.Id,
                IsSender=data.SenderId==userId,
                Fullname=sender.Firstname+ " "+sender.Lastname,
                Photo=sender.Image,
                Message=value.Message,
                SentDate=data.SentDate.ToString(),
            };
            await _context.Clients.Group($"group-{value.GroupId}")
       .SendAsync("ReceiveGroupMessage",dto );
            return Ok();
        }

        [HttpPost("AddMembersToGroup")]
        public async Task<IActionResult> AddMemberToGroup([FromBody] AddNewMembersGroupChatDto dto)
        {
          await _groupChatService.AddMembersToGroupChat(dto.GroupId,dto.MemberIds);
            return Ok();
        }

        [HttpGet("GetAllGroupMessages/{id}")]
        public async Task<IActionResult>GetGroupMessages(int id)
        {
            if(id == 0) { return BadRequest(); }
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var messages = await _groupChatService.GetAllGroupMessages(id);
                var list =new List<GroupChatMessageDto>();
            foreach (var m in messages)
            {
                
                     var user = await _userService.GetUserById(m.SenderId);
                list.Add(new GroupChatMessageDto { Id = m.Id, IsSender = m.SenderId == userId, Message = m.Content, SentDate = m.SentDate.ToString(), Photo = user.Image, Fullname = user.Firstname + " " + user.Lastname });
            }
            return Ok(list);
        } 
        [HttpGet("GetGroupChatDetails/{id}")]
        public async Task<IActionResult>GetGroupChatDetails(int id)
        {
            if(id == 0) { return BadRequest(); }
            var group=await _groupChatService.GetGroupChatDetails(id);
            var members = await _groupChatService.GetAllGroupMembersAsync(id);
            var membersDto=new List<GroupChatMemberDetailDto>();
            foreach (var item in members)
            {
                var user = await _userService.GetUserById(item.UserId);
                membersDto.Add(new GroupChatMemberDetailDto
                {
                    UserId = item.UserId,
                    Fullname = user.Firstname + " " + user.Lastname,
                    IsAdmin = item.Role == GroupRole.Admin,
                    JoinedAt = item.JoinedAt.ToString(),
                    ProfileImage = user.Image != null ? user.Image : " ",
                });
            }
            var dto = new GroupChatDetailsDto
            { 
                GroupName = group.Name,
                CreatedDate = group.CreatedDate.ToString(),
                GroupChatMembers = membersDto,
            };
            
            return Ok(dto);
        }
        
        [HttpGet("SearchFriendsForChat/{id}")]
        public async Task<IActionResult> SearchFriendsForChat(int id, [FromQuery] string key)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var friends =await _friendService.GetAllFriendsIdsAsync(userId);
            var data = (await _groupChatService.SearchFriendsForChat(friends, id, key)).Select(u => { return new GroupMemberPOStDto { UserId = u.Id, Fullname = u.Firstname + " " + u.Lastname, Username = u.UserName }; });


            return Ok(new {list=data});
        }



        // PUT api/<GroupChatController>/5
        [HttpPut("UpdateGroupName/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] string value)
        {
            await _groupChatService.UpdateGroupNameAsync(value,id);
            return Ok();
        }

        // DELETE api/<GroupChatController>/5
        [HttpDelete("RemoveGroupMember/{id}")]
        public async Task<IActionResult> RemoveGroupMember(int id, [FromBody] string userId)
        {
            await _groupChatService.RemoveGroupMember(id,userId);
            return Ok();
        }


    }
}
