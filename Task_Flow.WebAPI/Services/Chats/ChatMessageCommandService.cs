using Microsoft.AspNetCore.Identity;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Chats
{
    public class ChatMessageCommandService : IChatMessageCommandService
    {
        private readonly IChatService _chatService;
        private readonly IChatMessageService _chatMessageService;
        private readonly UserManager<CustomUser> _userManager;
        private readonly MessageEncryptionService _encryptionService;
        private readonly IChatRealtimeNotifier _notifier;

        public ChatMessageCommandService(
            IChatService chatService,
            IChatMessageService chatMessageService,
            UserManager<CustomUser> userManager,
            MessageEncryptionService encryptionService,
            IChatRealtimeNotifier notifier)
        {
            _chatService = chatService;
            _chatMessageService = chatMessageService;
            _userManager = userManager;
            _encryptionService = encryptionService;
            _notifier = notifier;
        }

        // Mesaj şifrələnmiş şəkildə saxlanılır
        public async Task<ServiceResult<object>> SendMessageAsync(string userId, ChatMessageDto dto)
        {
            var encrypted = _encryptionService.Encrypt(dto.Text);
            var friend = await _userManager.FindByEmailAsync(dto.FriendEmail);
            var chat = await _chatService.GetByRecieverAndSenderId(friend!.Id, userId);

            await _chatMessageService.AddAsync(new ChatMessage
            {
                Content = encrypted.CipherText,
                IV = encrypted.IV,
                SenderId = userId,
                SentDate = DateTime.UtcNow,
                ChatId = chat.Id,
                IsImage = dto.IsImage
            });

            await _notifier.NotifyMessageSentAsync(userId, friend.Email!);

            return ServiceResult<object>.Success(new { SenderId = userId, FriendId = friend.Id });
        }

        // Mesaj bazadan silinmir, "Deleted" statusu ilə işarələnir
        public async Task<ServiceResult<Empty>> DeleteMessageAsync(int messageId)
        {
            var message = await _chatMessageService.GetAsync(messageId);
            message.Status = ChatMessageTextResolver.DeletedStatus;
            await _chatMessageService.UpdateAsync(message);

            return ServiceResult<Empty>.Success(Empty.Value);
        }
    }
}
