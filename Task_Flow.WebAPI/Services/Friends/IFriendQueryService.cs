using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Friends
{
    /// <summary>
    /// İstifadəçi və dost siyahılarını oxuyan əməliyyatlar.
    /// </summary>
    public interface IFriendQueryService
    {
        Task<ServiceResult<List<FriendDto>>> GetAllUsersAsync(string userId);
        Task<ServiceResult<List<FriendDto>>> GetFriendsAsync(string userId);
        Task<ServiceResult<List<GroupChatFriendPOSTDto>>> GetFriendsForGroupChatAsync(string userId);
    }
}
