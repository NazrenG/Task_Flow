using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Enums;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class GroupChatService:IGroupChatService
    {
        private readonly IGroupChatDal _groupChatDal;
        private readonly IGroupChatMembersDal _groupChatMembersDal;
        private readonly IGroupChatMessageDal _groupChatMessageDal;
        private readonly MessageEncryptionService _encryptionService;
        private readonly IUserService _userService;

        public GroupChatService(IGroupChatDal groupChatDal,IGroupChatMembersDal groupChatMembersDal,IGroupChatMessageDal groupChatMessageDal,MessageEncryptionService messageEncryptionService, IUserService userService )
        {
            _groupChatDal = groupChatDal;
            _groupChatMembersDal = groupChatMembersDal;
            _groupChatMessageDal = groupChatMessageDal;
            _encryptionService = messageEncryptionService;
            _userService = userService;
        }

        public async Task AddMembersToGroupChat(int groupId, List<string> members)
        {
            var group = await _groupChatDal.GetById(g=>g.Id==groupId);
            foreach (var item in members)
            {
                var member = new GroupChatMembers{ GroupId = groupId, UserId = item, IsRemoved = false, JoinedAt = DateTime.UtcNow, Role = GroupRole.Member};
                group.Members.Add(member);
            }
            await _groupChatDal.Update(group);
        }

        public async Task AddMemberToGroupChatAsync(int groupId, string userId)
        {
            var group = await _groupChatDal.GetById(g => g.Id == groupId);
           
                var member = new GroupChatMembers { GroupId = groupId, UserId = userId, IsRemoved = false, JoinedAt = DateTime.UtcNow, Role = GroupRole.Member };
                group.Members.Add(member);
            
            await _groupChatDal.Update(group);
        }

        public async Task<int> CreateGroupChat(string adminId, string name)
        {
        
            var newGroup = new GroupChat
            {
                Name = name,
                CreatedDate = DateTime.UtcNow,
                AdminId = adminId,
            };

            var adminMember = new GroupChatMembers
            {
                UserId = adminId,
                Role = GroupRole.Admin, 
                JoinedAt = DateTime.UtcNow,
                IsRemoved = false,
                GroupChat = newGroup      
            };

            await _groupChatDal.Add(newGroup);
            newGroup.Members.Add(adminMember);
            await _groupChatDal.Update(newGroup);
            return newGroup.Id;
        }

        public async Task DeleteGroupChat(int groupId)
        {
         (await _groupChatDal.GetById(g => g.Id == groupId)).IsDeleted=true;
        }

        public async Task<List<GroupChatMembers>> GetAllGroupMembersAsync(int groupId)
        {
          return  await _groupChatMembersDal.GetAll(c=>c.GroupId==groupId&&!c.IsRemoved); ///member
        }

        public async Task<List<GroupChatMessage>> GetAllGroupMessages(int groupId)
        {
           var messages=await _groupChatMessageDal.GetAll(c=>c.GroupChatId==groupId);
          
             return messages
        .Select(m =>
        {
            m.Content = _encryptionService.Decrypt(m.Content,m.IV);
            return m;
        })
        .ToList(); 
        }

        public async Task<List<GroupChat>> GetAllUserGroupChatsAsync(string userId)
        {
            var memberships = await _groupChatMembersDal
        .GetAll(m => m.UserId == userId && !m.IsRemoved);

            var groupIds = memberships
                .Select(m => m.GroupId)
                .Distinct()
                .ToList();

            var groups = new List<GroupChat>();

            foreach (var groupId in groupIds)
            {
                var group = await _groupChatDal.GetById(g => g.Id == groupId);
                if (group != null)
                    groups.Add(group);
            }

            return groups;
        }

        public Task<GroupChatMembers> GetGroupAdminAsync(int groupId)
        {
          return _groupChatMembersDal.GetById(c=>c.GroupId==groupId&& c.Role==GroupRole.Admin);
        }

        public async Task<GroupChat> GetGroupChatDetails(int groupId)
        {
            return await _groupChatDal.GetById(c=>c.Id==groupId);   
        }

        public async Task<bool> IsGroupAdminAsync(string userId,int groupId)
        {
           return (await _groupChatDal.GetById(g=>g.Id==groupId)).AdminId==userId;
        }

        public async Task RemoveGroupMember(int groupId, string userId  )
        {

            var member =await _groupChatMembersDal.GetById(m=>m.GroupId==groupId&&m.UserId==userId&&!m.IsRemoved);  
            member.IsRemoved = true;
            await _groupChatMembersDal.Update(member);  
        }

        public async Task<List<CustomUser>> SearchFriendsForChat(List<string> friendIds, int groupId, string key)
        {
            if (string.IsNullOrWhiteSpace(key) || !friendIds.Any())
                return new List<CustomUser>();

            var memberIds = (await _groupChatMembersDal.GetAll(m =>
           m.GroupId == groupId && !m.IsRemoved
       ))
       .Select(m => m.UserId)
       .ToHashSet();

            var availableFriendIds = friendIds
       .Where(id => !memberIds.Contains(id))
       .ToList();

            if (!availableFriendIds.Any())
                return new List<CustomUser>();

            var users = await _userService.GetUsers();

            var result = users
                .Where(u =>
                    availableFriendIds.Contains(u.Id) &&
                 
                        u.UserName.Contains(key, StringComparison.OrdinalIgnoreCase)
                    
                )
                .Select(u => u)
                .ToList();

            return result;
        }


        public async Task<GroupChatMessage> SendMessageToGroup(int groupId, string senderId, string message)
        {

            var chipperText= _encryptionService.Encrypt(message);
            var data = new GroupChatMessage { GroupChatId = groupId, SenderId = senderId, Content = chipperText.CipherText, IsDeleted = false, SentDate = DateTime.Now, IV = chipperText.IV };
            await _groupChatMessageDal.Add(data);
            return data;
        }

        public async Task UpdateGroupNameAsync(string groupName, int groupId)
        {
            var group = await _groupChatDal.GetById(c => c.Id == groupId);
            group.Name = groupName;
            await _groupChatDal.Update(group);
        }
    }
}
