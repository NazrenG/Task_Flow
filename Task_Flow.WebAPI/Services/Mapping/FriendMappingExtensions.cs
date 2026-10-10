using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Services.Mapping
{
    public static class FriendMappingExtensions
    {
        // "İstifadəçilər" siyahısı: dostluq və göndərilmiş sorğu statusu ilə
        public static FriendDto ToUserListItem(this CustomUser user, bool hasRequestPending, bool isFriend)
        {
            return new FriendDto
            {
                Id = user.Id,
                FriendName = user.UserName,
                IsOnline = user.IsOnline,
                FriendPhoto = user.Image,
                FriendEmail = user.Email,
                HasRequestPending = hasRequestPending,
                IsFriend = isFriend
            };
        }

        public static FriendDto ToFriendDto(this Friend friend, bool isMutual)
        {
            var user = friend.UserFriend!;
            return new FriendDto
            {
                FriendName = user.Firstname + " " + user.Lastname,
                FriendEmail = user.Email,
                FriendOccupation = user.Occupation,
                FriendPhone = user.PhoneNumber,
                FriendPhoto = user.Image,
                IsOnline = user.IsOnline,
                CheckFriend = isMutual
            };
        }

        public static GroupChatFriendPOSTDto ToGroupChatFriendDto(this Friend friend)
        {
            return new GroupChatFriendPOSTDto
            {
                Fullname = friend.UserFriend!.Firstname + " " + friend.UserFriend.Lastname,
                Id = friend.UserFriendId!
            };
        }
    }
}
