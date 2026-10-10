using Microsoft.AspNetCore.Identity;
using Task_Flow.Business.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Friends
{
    public class FriendCommandService : IFriendCommandService
    {
        private const string FriendNotFoundMessage = "friend not found";

        private readonly IFriendService _friendService;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IRequestNotificationService _requestNotificationService;

        public FriendCommandService(
            IFriendService friendService,
            UserManager<CustomUser> userManager,
            IRequestNotificationService requestNotificationService)
        {
            _friendService = friendService;
            _userManager = userManager;
            _requestNotificationService = requestNotificationService;
        }

        public async Task<ServiceResult<Friend>> FollowAsync(string userId, SendFollowFriendDto value)
        {
            var friend = new Friend
            {
                UserFriendId = value.FriendId,
                UserId = userId
            };
            await _friendService.Add(friend);

            return ServiceResult<Friend>.Success(friend);
        }

        public async Task<ServiceResult<object>> UnfollowAsync(string userId, string friendEmail)
        {
            var friendUser = await _userManager.FindByEmailAsync(friendEmail);
            if (friendUser == null)
            {
                return FriendNotFound();
            }

            var friendship = await _friendService.GetFriendByUserAndFriendId(userId, friendUser.Id);
            if (friendship == null)
            {
                return FriendNotFound();
            }

            await _friendService.Delete(friendship);

            return ServiceResult<object>.Success(new { message = "accept request succesfuly" });
        }

        public async Task<ServiceResult<Empty>> DeleteRequestAsync(int requestId)
        {
            var request = await _requestNotificationService.GetRequestNotificationById(requestId);
            await _requestNotificationService.Delete(request);

            return ServiceResult<Empty>.Success(Empty.Value);
        }

        private static ServiceResult<object> FriendNotFound()
        {
            return ServiceResult<object>.Success(new { message = FriendNotFoundMessage });
        }
    }
}
