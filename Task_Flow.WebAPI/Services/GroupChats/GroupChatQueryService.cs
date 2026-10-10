using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.GroupChats
{
    public class GroupChatQueryService : IGroupChatQueryService
    {
        private readonly IGroupChatService _groupChatService;
        private readonly IUserService _userService;
        private readonly IFriendService _friendService;

        public GroupChatQueryService(
            IGroupChatService groupChatService,
            IUserService userService,
            IFriendService friendService)
        {
            _groupChatService = groupChatService;
            _userService = userService;
            _friendService = friendService;
        }

        public async Task<ServiceResult<object>> GetAdminAsync(int groupId)
        {
            var admin = await _groupChatService.GetGroupAdminAsync(groupId);
            if (admin == null)
            {
                return ServiceResult<object>.NotFound();
            }

            return ServiceResult<object>.Success(new { Admin = admin });
        }

        public async Task<ServiceResult<object>> GetUserGroupChatsAsync(string? userId)
        {
            var groupChats = await _groupChatService.GetAllUserGroupChatsAsync(userId!);
            return ServiceResult<object>.Success(new { List = groupChats });
        }

        public async Task<ServiceResult<List<GroupChatMessageDto>>> GetMessagesAsync(int groupId, string? currentUserId)
        {
            var messages = await _groupChatService.GetAllGroupMessages(groupId);
            var result = new List<GroupChatMessageDto>();

            foreach (var message in messages)
            {
                var sender = await _userService.GetUserById(message.SenderId);
                result.Add(message.ToGroupChatMessageDto(sender, currentUserId, message.Content));
            }

            return ServiceResult<List<GroupChatMessageDto>>.Success(result);
        }

        public async Task<ServiceResult<GroupChatDetailsDto>> GetDetailsAsync(int groupId, string? currentUserId)
        {
            var group = await _groupChatService.GetGroupChatDetails(groupId);
            var members = await _groupChatService.GetAllGroupMembersAsync(groupId);

            var memberDtos = new List<GroupChatMemberDetailDto>();
            foreach (var member in members)
            {
                var user = await _userService.GetUserById(member.UserId);
                memberDtos.Add(member.ToMemberDetailDto(user));
            }

            return ServiceResult<GroupChatDetailsDto>.Success(new GroupChatDetailsDto
            {
                GroupName = group.Name,
                CreatedDate = group.CreatedDate.ToString(),
                GroupChatMembers = memberDtos,
                IsCurrentuserAdmin = await _groupChatService.IsGroupAdminAsync(currentUserId!, groupId)
            });
        }

        // Yalnız istifadəçinin dostları arasında, qrupda olmayanlar axtarılır
        public async Task<ServiceResult<object>> SearchFriendsForChatAsync(int groupId, string? userId, string key)
        {
            var friendIds = await _friendService.GetAllFriendsIdsAsync(userId!);
            var candidates = await _groupChatService.SearchFriendsForChat(friendIds, groupId, key);

            return ServiceResult<object>.Success(new { list = candidates.Select(u => u.ToGroupMemberCandidateDto()).ToList() });
        }
    }
}
