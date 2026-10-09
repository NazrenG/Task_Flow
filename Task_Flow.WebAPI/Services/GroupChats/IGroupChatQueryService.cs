using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.GroupChats
{
    /// <summary>
    /// Qrup çatlarını, mesajlarını və üzvlərini oxuyan əməliyyatlar.
    /// </summary>
    public interface IGroupChatQueryService
    {
        Task<ServiceResult<object>> GetAdminAsync(int groupId);
        Task<ServiceResult<object>> GetUserGroupChatsAsync(string? userId);
        Task<ServiceResult<List<GroupChatMessageDto>>> GetMessagesAsync(int groupId, string? currentUserId);
        Task<ServiceResult<GroupChatDetailsDto>> GetDetailsAsync(int groupId, string? currentUserId);
        Task<ServiceResult<object>> SearchFriendsForChatAsync(int groupId, string? userId, string key);
    }
}
