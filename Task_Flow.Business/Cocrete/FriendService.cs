﻿using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class FriendService:IFriendService
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

        public async Task<Friend> GetFriendsById(int id)
        {
            return await dal.GetById(f => f.Id == id);
        }

        public async Task<List<Friend>> GetFriends(string userId)
        {
            return await dal.GetAllFriends(userId);
        }

        public async Task Update(Friend friend)
        {
            await dal.Update(friend);
        }
    }
}
