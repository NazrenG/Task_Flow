using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface IFriendService
    {
        Task<List<Friend>> GetFriends(string userId);
        Task<Friend> GetFriendByUserAndFriendId(string userId,string friendId);
        Task Add(Friend friend);
        Task Update(Friend friend);
        Task Delete(Friend friend);
        Task<bool>CheckFriendship(string userId, string friendId);
        Task<bool> MutualFriends(string userId, string friendId);
        Task<List<string>> GetAllFriendsIdsAsync(string userId);
    }
}
