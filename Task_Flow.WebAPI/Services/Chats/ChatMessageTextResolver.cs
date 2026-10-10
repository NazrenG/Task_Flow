using Task_Flow.Business.Cocrete;
using Task_Flow.Entities.Models;

namespace Task_Flow.WebAPI.Services.Chats
{
    public class ChatMessageTextResolver : IChatMessageTextResolver
    {
        public const string DeletedStatus = "Deleted";
        private const string DeletedMessageText = "This message was deleted!";

        private readonly MessageEncryptionService _encryptionService;

        public ChatMessageTextResolver(MessageEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public string Resolve(ChatMessage message)
        {
            if (message.Status == DeletedStatus)
            {
                return DeletedMessageText;
            }

            // IV varsa mesaj şifrələnib, yoxdursa şifrələmədən əvvəlki köhnə mesajdır
            return string.IsNullOrEmpty(message.IV)
                ? message.Content!
                : _encryptionService.Decrypt(message.Content!, message.IV);
        }
    }
}
