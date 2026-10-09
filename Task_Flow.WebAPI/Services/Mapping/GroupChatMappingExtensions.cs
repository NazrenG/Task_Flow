using Task_Flow.Entities.Enums;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Services.Mapping
{
    public static class GroupChatMappingExtensions
    {
        private const string EmptyProfileImage = " ";

        public static GroupChatMessageDto ToGroupChatMessageDto(
            this GroupChatMessage message, CustomUser sender, string? currentUserId, string text)
        {
            return new GroupChatMessageDto
            {
                Id = message.Id,
                IsSender = message.SenderId == currentUserId,
                Fullname = sender.Firstname + " " + sender.Lastname,
                Photo = sender.Image!,
                Message = text,
                SentDate = message.SentDate.ToString()
            };
        }

        public static GroupChatMemberDetailDto ToMemberDetailDto(this GroupChatMembers member, CustomUser user)
        {
            return new GroupChatMemberDetailDto
            {
                UserId = member.UserId,
                Fullname = user.Firstname + " " + user.Lastname,
                IsAdmin = member.Role == GroupRole.Admin,
                JoinedAt = member.JoinedAt.ToString(),
                ProfileImage = user.Image ?? EmptyProfileImage
            };
        }

        public static GroupMemberPOStDto ToGroupMemberCandidateDto(this CustomUser user)
        {
            return new GroupMemberPOStDto
            {
                UserId = user.Id,
                Fullname = user.Firstname + " " + user.Lastname,
                Username = user.UserName!
            };
        }
    }
}
