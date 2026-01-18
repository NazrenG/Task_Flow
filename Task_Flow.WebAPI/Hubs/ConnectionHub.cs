using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.WebAPI.Hubs
{
        [Authorize]
    public class ConnectionHub : Hub
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<CustomUser> _userManager;
        public ConnectionHub(IUserService userService, IHttpContextAccessor contextAccessor,UserManager<CustomUser>userManager)
        {
            _userService = userService;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public async Task SendOnlineStatus(string userId, bool isOnline)
        {
            var Id = userId;
            if (userId != null)
            {
                var user = await _userService.GetUserById(userId);
                if (user != null)
                {
                    user.IsOnline = isOnline;
                    await _userService.Update(user);
                    await Clients.All.SendAsync("ReceiveOnlineStatus", userId, isOnline);
                }
               
            }
        }
      
        public async Task NotifyProjectUpdate(int projectId)
        {
            await Clients.All.SendAsync("ReceiveProjectUpdate", projectId);
        }

        public override async Task OnConnectedAsync()
        {
            //var user = await _userManager.GetUserAsync(Context.User);
            ////if (userId != null)
            ////{
            ////    var user = await _userService.GetUserById(userId);
            ////    if (user != null)
            ////    {
            ////        user.IsOnline = true;
            ////        await _userService.Update(user);
            ////        await Clients.All.SendAsync("ReceiveConnectInfo", $"{user.UserName} has connected");
            ////    }
            ////}
            //if (user != null)
            //{
            //    //await Clients.All.SendAsync("ReceiveConnectInfo", $"{user.UserName} has connected");
            //    await Clients.All.SendAsync("UserStatusChange", user.Email, true);
            //    user.IsOnline = true;
            //    user.LastLoginDate = DateTime.UtcNow;
            //    await _userService.Update(user);
            //}
            //await base.OnConnectedAsync();
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (userName != null)
            {
                var currentUser = await _userService.GetOneUSerByUsername(userName!);

                if (currentUser != null)
                {
                    currentUser.IsOnline = true;

                    await _userService.Update(currentUser);

                    //await Clients.All.SendAsync("UpdateUserActivity");
                    await Clients.Others.SendAsync("UpdateUserActivity");
                }

            }
        }

        public async Task GetMessages(string receiverId, string senderId)
        {
            //var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            //CustomUser currentUser = null;
            //if (userName != null)
            //{
            //   currentUser = await _userService.GetOneUSerByUsername(userName!);
            //}
            //else { return; }

                var friend=await _userManager.FindByIdAsync(senderId);
            //var mail = currentUser.Id == senderId ? "" : friend.Email;
            await Clients.User(receiverId).SendAsync("ReceiveMessages2", friend.Email);                                                        
        }
        public async Task UpdateOwnProjectList()
        {
            await Clients.All.SendAsync("UpdateProjectList");

        }
        public async Task SendFollow(string id)
        {
            await Clients.User(id).SendAsync("UpdateProfileRequestList");
        }
        public async Task JoinGroup(int groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"group-{groupId}");
        }
        public async Task LeaveGroup(int groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"group-{groupId}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (user != null)
            {

                user.IsOnline = false;
                await _userService.Update(user);
                //await Clients.All.SendAsync("UpdateProjectList");
                await Clients.Others.SendAsync("UpdateUserActivity");

            }
            await base.OnDisconnectedAsync(exception);
        }

        
    }
}
