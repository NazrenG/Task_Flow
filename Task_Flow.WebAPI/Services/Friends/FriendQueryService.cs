using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.NotificationCenter;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Friends
{
    public class FriendQueryService : IFriendQueryService
    {
        private readonly IFriendService _friendService;
        private readonly IUserService _userService;
        private readonly IRequestNotificationService _requestNotificationService;

        public FriendQueryService(
            IFriendService friendService,
            IUserService userService,
            IRequestNotificationService requestNotificationService)
        {
            _friendService = friendService;
            _userService = userService;
            _requestNotificationService = requestNotificationService;
        }

        // Bütün digər istifadəçilər: əvvəlcə online olanlar
        public async Task<ServiceResult<List<FriendDto>>> GetAllUsersAsync(string userId)
        {
            var sentRequests = await _requestNotificationService.GetNotificationsBySenderId(userId);
            var friends = await _friendService.GetFriends(userId);
            var users = await _userService.GetUsers();

            var result = users
                .Where(u => u.Id != userId)
                .OrderByDescending(u => u.IsOnline)
                .Select(u => u.ToUserListItem(
                    hasRequestPending: HasPendingFriendRequest(sentRequests, u.Id),
                    isFriend: IsFriend(friends, u.Id)))
                .ToList();

            return ServiceResult<List<FriendDto>>.Success(result);
        }

        public async Task<ServiceResult<List<FriendDto>>> GetFriendsAsync(string userId)
        {
            var friends = await _friendService.GetFriends(userId);
            var result = new List<FriendDto>();

            foreach (var friend in friends)
            {
                var isMutual = await _friendService.CheckFriendship(friend.UserId!, friend.UserFriendId!);
                result.Add(friend.ToFriendDto(isMutual));
            }

            return ServiceResult<List<FriendDto>>.Success(result);
        }

        public async Task<ServiceResult<List<GroupChatFriendPOSTDto>>> GetFriendsForGroupChatAsync(string userId)
        {
            var friends = await _friendService.GetFriends(userId);
            return ServiceResult<List<GroupChatFriendPOSTDto>>.Success(
                friends.Select(f => f.ToGroupChatFriendDto()).ToList());
        }

        private static bool HasPendingFriendRequest(IEnumerable<RequestNotification> sentRequests, string receiverId)
        {
            return sentRequests.Any(r =>
                r.ReceiverId == receiverId &&
                r.NotificationType == RequestNotificationTypes.FriendRequest &&
                !r.IsAccepted);
        }

        private static bool IsFriend(IEnumerable<Friend> friends, string userId)
        {
            return friends.Any(f => f.UserId == userId || f.UserFriendId == userId);
        }
    }
}
