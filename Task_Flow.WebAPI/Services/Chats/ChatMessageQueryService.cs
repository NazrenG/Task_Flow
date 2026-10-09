using Microsoft.AspNetCore.Identity;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Chats
{
    public class ChatMessageQueryService : IChatMessageQueryService
    {
        private readonly IChatService _chatService;
        private readonly IChatMessageService _chatMessageService;
        private readonly IUserService _userService;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IChatMessageTextResolver _textResolver;

        public ChatMessageQueryService(
            IChatService chatService,
            IChatMessageService chatMessageService,
            IUserService userService,
            UserManager<CustomUser> userManager,
            IChatMessageTextResolver textResolver)
        {
            _chatService = chatService;
            _chatMessageService = chatMessageService;
            _userService = userService;
            _userManager = userManager;
            _textResolver = textResolver;
        }

        public async Task<ServiceResult<object>> GetConversationAsync(string userId, string friendEmail)
        {
            if (string.IsNullOrEmpty(friendEmail))
            {
                return ServiceResult<object>.Success(new { List = new List<UserMessageDto>() });
            }

            var friend = await _userManager.FindByEmailAsync(friendEmail);
            var chat = await GetOrCreateChatAsync(userId, friend!.Id);
            var messages = await _chatMessageService.GetAllByChatId(chat.Id);

            var result = new List<UserMessageDto>();
            foreach (var message in messages)
            {
                var sender = await _userService.GetUserById(message.SenderId!);
                result.Add(new UserMessageDto
                {
                    IsOnline = sender.IsOnline,
                    IsSender = sender.Id == userId,
                    Fullname = sender.Firstname + " " + sender.Lastname,
                    Message = _textResolver.Resolve(message),
                    Photo = sender.Image!,
                    Status = message.Status,
                    SentDate = message.SentDate,
                    MessageId = message.Id
                });
            }

            return ServiceResult<object>.Success(new { List = result });
        }

        // İki istifadəçi arasında çat hələ yoxdursa, yaradılır
        private async Task<Chat> GetOrCreateChatAsync(string userId, string friendId)
        {
            var chat = await _chatService.GetByRecieverAndSenderId(friendId, userId);
            if (chat != null)
            {
                return chat;
            }

            chat = new Chat { SenderId = userId, ReceiverId = friendId, Messages = new List<ChatMessage>() };
            await _chatService.AddAsync(chat);
            return chat;
        }
    }
}
