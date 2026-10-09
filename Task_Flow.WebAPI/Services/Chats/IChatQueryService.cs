using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Chats
{
    /// <summary>
    /// Çat siyahısı və istifadəçiyə gələn son mesajlar.
    /// </summary>
    public interface IChatQueryService
    {
        Task<ServiceResult<object>> GetChatListAsync(string userId);
        Task<ServiceResult<object>> GetIncomingMessagesAsync(string userId);
        Task<ServiceResult<object>> GetLatestMessagesAsync(string userId);
        Task<ServiceResult<object>> GetActiveChatCountAsync(string? userId);
    }
}
