using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Chats
{
    public class ChatQueryService : IChatQueryService
    {
        private readonly IUserService _userService;
        private readonly IFriendService _friendService;
        private readonly IChatService _chatService;
        private readonly IChatMessageService _chatMessageService;
        private readonly IChatMessageTextResolver _textResolver;

        public ChatQueryService(
            IUserService userService,
            IFriendService friendService,
            IChatService chatService,
            IChatMessageService chatMessageService,
            IChatMessageTextResolver textResolver)
        {
            _userService = userService;
            _friendService = friendService;
            _chatService = chatService;
            _chatMessageService = chatMessageService;
            _textResolver = textResolver;
        }

        // Qarşılıqlı dostlarla çatlar, son mesajla birlikdə.
        // Son mesajı istifadəçinin özü göndərdiyi çatlar əvvəl gəlir.
        public async Task<ServiceResult<object>> GetChatListAsync(string userId)
        {
            var mutualFriends = await GetMutualFriendsAsync(userId);

            var chats = new List<FriendForMessageDto>();
            foreach (var friend in mutualFriends)
            {
                chats.Add(await BuildChatItemAsync(userId, friend.UserFriendId!));
            }

            var ordered = chats.OrderBy(c => c.isReciever).ToList();
            return ServiceResult<object>.Success(new { List = ordered });
        }

        // Son mesajı başqası göndərən çatlar
        public async Task<ServiceResult<object>> GetIncomingMessagesAsync(string userId)
        {
            var chats = await _chatService.GetAllChatByUserId(userId);
            if (chats == null)
            {
                return ServiceResult<object>.Success(new { Resut = false });
            }

            var messages = new List<AllMessagesDto>();
            foreach (var chat in chats)
            {
                var lastMessage = await _chatMessageService.GetLatestMessageByChatIdAsync(chat.Id);
                if (!IsReceivedBy(lastMessage, userId))
                {
                    continue;
                }

                var sender = await _userService.GetUserById(lastMessage!.SenderId!);
                messages.Add(new AllMessagesDto
                {
                    FriendName = sender.Firstname!,
                    FriendLastname = sender.Lastname!,
                    FriendImg = sender.Image!,
                    Message = lastMessage.Content!,
                    SentDate = lastMessage.SentDate.ToShortDateString()
                });
            }

            return ServiceResult<object>.Success(new { Result = true, List = messages });
        }

        private async Task<List<Friend>> GetMutualFriendsAsync(string userId)
        {
            var friends = await _friendService.GetFriends(userId);
            var mutualFriends = new List<Friend>();

            foreach (var friend in friends)
            {
                if (await _friendService.MutualFriends(friend.UserId!, friend.UserFriendId!))
                {
                    mutualFriends.Add(friend);
                }
            }

            return mutualFriends;
        }

        private async Task<FriendForMessageDto> BuildChatItemAsync(string userId, string friendId)
        {
            var chat = await _chatService.GetByRecieverAndSenderId(userId, friendId);
            var friendUser = await _userService.GetUserById(friendId);

            ChatMessage? latestMessage = null;
            if (chat != null)
            {
                latestMessage = await _chatMessageService.GetLatestMessageByChatIdAsync(chat.Id);
            }

            return new FriendForMessageDto
            {
                FriendFullname = friendUser.Firstname + " " + friendUser.Lastname,
                FriendEmail = friendUser.Email,
                FriendImg = friendUser.Image,
                isReciever = IsReceivedBy(latestMessage, userId),
                RecentMessage = latestMessage == null ? "" : _textResolver.Resolve(latestMessage),
                IsOnline = friendUser.IsOnline
            };
        }

        private static bool IsReceivedBy(ChatMessage? message, string userId)
        {
            return message != null && message.SenderId != userId;
        }
    }
}
