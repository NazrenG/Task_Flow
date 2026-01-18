using Microsoft.EntityFrameworkCore;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class FriendService : IFriendService
    {
        private readonly IFriendDal dal;

        public FriendService(IFriendDal friendDal)
        {
            dal = friendDal;
        }

        public async Task Add(Friend friend)
        {
            await dal.Add(friend);
        }

        public async Task Delete(Friend friend)
        {
            await dal.Delete(friend);
        }



        public async Task<List<Friend>> GetFriends(string userId)
        {
            return await dal.GetAllFriends(userId);
        }

        public async Task Update(Friend friend)
        {
            await dal.Update(friend);
        }

        public async Task<Friend> GetFriendByUserAndFriendId(string userId, string friendId)
        {
            var friends = await dal.GetAllFriends(userId);
            return friends.FirstOrDefault(f => f.UserFriendId == friendId);
        }

        public async Task<bool> CheckFriendship(string userId, string friendId)

        {

            var list = await dal.GetAll();
            var count = list.Where(f => (f.UserId == userId && f.UserFriendId == friendId && f.IsFriend == true) || (f.UserId == friendId && f.UserFriendId == userId && f.IsFriend == true)).Count();
            return count == 2;
        }

        public async Task<bool> MutualFriends(string userId, string friendId)
        {
            var list = await dal.GetAll();
            var count = list.Where(f =>
           (f.UserId == userId && f.UserFriendId == friendId ) ||
           (f.UserId == friendId && f.UserFriendId == userId))
       .Count();

            return count == 2;
        }

        public async Task<List<string>> GetAllFriendsIdsAsync(string userId)
        {
            var friends = await dal.GetAll(f =>
      f.IsFriend &&
      !f.HasRequestPending &&
      (f.UserId == userId || f.UserFriendId == userId)
  );

            return friends
                .Select(f => f.UserId == userId ? f.UserFriendId : f.UserId)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();
        }
    }
}
