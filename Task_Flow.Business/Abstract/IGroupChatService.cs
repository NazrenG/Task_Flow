using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface IGroupChatService
    {
        Task<GroupChatMembers> GetGroupAdminAsync(int groupId);
        Task<bool> IsGroupAdminAsync(string userId,int groupId);
        Task<List<GroupChatMembers>> GetAllGroupMembersAsync(int groupId);
        Task AddMembersToGroupChat(int groupId, List<string> members);
        Task<int> CreateGroupChat(string adminId,string name);
        Task<List<GroupChat>> GetAllUserGroupChatsAsync(string userId);
        Task<List<GroupChatMessage>> GetAllGroupMessages(int groupId);
        Task DeleteGroupChat(int groupId);
        Task<GroupChatMessage> SendMessageToGroup(int groupId,string senderId, string message);
        Task<GroupChat> GetGroupChatDetails(int groupId);   
        Task UpdateGroupNameAsync(string groupName, int groupId);
        Task RemoveGroupMember(int groupId,string userId);
        Task AddMemberToGroupChatAsync(int groupId,string userId);
        Task<List<CustomUser>> SearchFriendsForChat(List<string>friendIds,int groupId,string key);

    }
}
