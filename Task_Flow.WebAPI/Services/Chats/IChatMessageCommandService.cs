using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Chats
{
    /// <summary>
    /// Şəxsi çatda mesaj göndərmək və silmək.
    /// </summary>
    public interface IChatMessageCommandService
    {
        Task<ServiceResult<object>> SendMessageAsync(string userId, ChatMessageDto dto);
        Task<ServiceResult<Empty>> DeleteMessageAsync(int messageId);
    }
}
