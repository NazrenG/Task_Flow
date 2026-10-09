using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Chats
{
    /// <summary>
    /// İki istifadəçi arasındakı yazışmanı oxuyur.
    /// </summary>
    public interface IChatMessageQueryService
    {
        Task<ServiceResult<object>> GetConversationAsync(string userId, string friendEmail);
    }
}
