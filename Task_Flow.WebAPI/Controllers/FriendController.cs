using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService friendService;
        private readonly IUserService _userService;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IRequestNotificationService _requestNotificationService;
        private readonly IHubContext<ConnectionHub> hubContext;

        public FriendController(IFriendService friendService, IUserService userService, UserManager<CustomUser> userManager, IRequestNotificationService requestNotificationService, IHubContext<ConnectionHub> hubContext)
        {
            this.friendService = friendService;
            _userService = userService;
            _userManager = userManager;
            _requestNotificationService = requestNotificationService;
            this.hubContext = hubContext;
        }

        [Authorize]
        [HttpGet("AllUser")]
        public async Task<IActionResult> GetAllUsers()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var myrequests = await _requestNotificationService.GetNotificationsBySenderId(userId);

            var allfriends = await friendService.GetFriends(userId);

            var allusers = await _userService.GetUsers();
            var users = allusers.Where(u => u.Id != userId)
            .OrderByDescending(u => u.IsOnline)
            .Select(u => new FriendDto
            {
                Id = u.Id,
                FriendName = u.UserName,
                IsOnline = u.IsOnline,
                FriendPhoto = u.Image,
                FriendEmail = u.Email,
                HasRequestPending = (myrequests.FirstOrDefault(r => r.ReceiverId == u.Id&& r.NotificationType == "FriendRequest"&&!r.IsAccepted) != null),
                IsFriend = allfriends.FirstOrDefault(f => f.UserId == u.Id || f.UserFriendId == u.Id) != null
            }).ToList();

            return Ok(users);

           

        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            try
            {
                var request = await _requestNotificationService.GetRequestNotificationById(id); 
               await _requestNotificationService.Delete(request);
                return Ok();
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

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var list = await friendService.GetFriends(userId);
            var result = new List<FriendDto>();

            foreach (var p in list)
            {
                var check = await friendService.CheckFriendship(p.UserId, p.UserFriendId);
                result.Add(new FriendDto
                {
                    FriendName = p.UserFriend.Firstname + " " + p.UserFriend.Lastname,
                    FriendEmail = p.UserFriend.Email,
                    FriendOccupation = p.UserFriend.Occupation,
                    FriendPhone = p.UserFriend.PhoneNumber,
                    FriendPhoto = p.UserFriend.Image,
                    IsOnline = p.UserFriend.IsOnline,
                    CheckFriend = check,
                });
            }
            return Ok(result);
        } 
        
        [Authorize]
        [HttpGet("AllFriendsForGroupChat")]
        public async Task<IActionResult> GetFriends()
        {

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var list = await friendService.GetFriends(userId);
            var result = new List<GroupChatFriendPOSTDto>();

            foreach (var p in list)
            {
                var check = await friendService.CheckFriendship(p.UserId, p.UserFriendId);
                result.Add(new GroupChatFriendPOSTDto
                {
                   Fullname= p.UserFriend.Firstname + " " + p.UserFriend.Lastname,
                   Id=p.UserFriendId,
                   
                });
            }
            return Ok(result);
        }



        //  POST api/<FriendController>
        [Authorize]
        [HttpPost("NewFriend")]
        public async Task<IActionResult> Post([FromBody] SendFollowFriendDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var item = new Friend
            {
                UserFriendId = value.FriendId,
                UserId = userId,
            };
            await friendService.Add(item);
            return Ok(item);
        }


        [Authorize]
        [HttpDelete("UnFollow/{friendMail}")]
        public async Task<IActionResult> DeleteFriend(string friendMail)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new { message = "user not found" });
            }
            var friend = await _userManager.FindByEmailAsync(friendMail);
            if (friend == null) { return Ok(new { message = "friend not found" }); }
            var existFriend = await friendService.GetFriendByUserAndFriendId(userId, friend.Id);
            if (existFriend == null) return Ok(new { message = "friend not found" });
            await friendService.Delete(existFriend);
            return Ok(new { message = "accept request succesfuly" });

        }
    }
}
