using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.GroupChats
{
    public class GroupChatCommandService : IGroupChatCommandService
    {
        private readonly IGroupChatService _groupChatService;
        private readonly IUserService _userService;
        private readonly IGroupChatRealtimeNotifier _notifier;

        public GroupChatCommandService(
            IGroupChatService groupChatService,
            IUserService userService,
            IGroupChatRealtimeNotifier notifier)
        {
            _groupChatService = groupChatService;
            _userService = userService;
            _notifier = notifier;
        }

        public async Task<ServiceResult<Empty>> CreateGroupAsync(string? userId, CreateChatGroupDto value)
        {
            var groupId = await _groupChatService.CreateGroupChat(userId!, value.GroupName);
            if (value.Members.Count > 0)
            {
                await _groupChatService.AddMembersToGroupChat(groupId, value.Members);
            }

            return Ok();
        }

        // Mesaj saxlanılır və qrupun bütün üzvlərinə real vaxtda göndərilir
        public async Task<ServiceResult<Empty>> SendMessageAsync(string? userId, SendMessageToGroupDto value)
        {
            var message = await _groupChatService.SendMessageToGroup(value.GroupId, userId!, value.Message);
            var sender = await _userService.GetUserById(userId!);

            var dto = message.ToGroupChatMessageDto(sender, userId, value.Message);
            await _notifier.SendGroupMessageAsync(value.GroupId, dto);

            return Ok();
        }

        public async Task<ServiceResult<Empty>> AddMembersAsync(AddNewMembersGroupChatDto dto)
        {
            await _groupChatService.AddMembersToGroupChat(dto.GroupId, dto.MemberIds);
            return Ok();
        }

        public async Task<ServiceResult<Empty>> RenameGroupAsync(int groupId, string name)
        {
            await _groupChatService.UpdateGroupNameAsync(name, groupId);
            return Ok();
        }

        public async Task<ServiceResult<Empty>> RemoveMemberAsync(int groupId, string memberId, string? currentUserId)
        {
            await _groupChatService.RemoveGroupMember(groupId, memberId);
            await _notifier.NotifyGroupMembersChangedAsync(currentUserId!);
            return Ok();
        }

        public async Task<ServiceResult<Empty>> DeleteGroupAsync(int groupId, string? userId)
        {
            await _groupChatService.DeleteGroupChat(groupId);
            await _notifier.NotifyGroupListChangedAsync(userId!);
            return Ok();
        }

        public async Task<ServiceResult<Empty>> ExitGroupAsync(int groupId, string? userId)
        {
            await _groupChatService.ExitGroup(groupId, userId!);
            await _notifier.NotifyGroupMembersChangedAsync(userId!);
            await _notifier.NotifyGroupListChangedAsync(userId!);
            return Ok();
        }

        private static ServiceResult<Empty> Ok() => ServiceResult<Empty>.Success(Empty.Value);
    }
}
