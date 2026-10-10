using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.GroupChats
{
    /// <summary>
    /// Qrup çatını yaradan, dəyişən və mesaj göndərən əməliyyatlar.
    /// </summary>
    public interface IGroupChatCommandService
    {
        Task<ServiceResult<Empty>> CreateGroupAsync(string? userId, CreateChatGroupDto value);
        Task<ServiceResult<Empty>> SendMessageAsync(string? userId, SendMessageToGroupDto value);
        Task<ServiceResult<Empty>> AddMembersAsync(AddNewMembersGroupChatDto dto);
        Task<ServiceResult<Empty>> RenameGroupAsync(int groupId, string name);
        Task<ServiceResult<Empty>> RemoveMemberAsync(int groupId, string memberId, string? currentUserId);
        Task<ServiceResult<Empty>> DeleteGroupAsync(int groupId, string? userId);
        Task<ServiceResult<Empty>> ExitGroupAsync(int groupId, string? userId);
    }
}
