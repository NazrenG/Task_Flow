using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Messages
{
    /// <summary>
    /// Köhnə Message entity-si ilə işləyən mesajlar (şəxsi çatdan əvvəlki model).
    /// </summary>
    public interface IMessageAppService
    {
        Task<ServiceResult<List<object>>> GetReceivedMessagesAsync(string userId);
        Task<ServiceResult<Message>> AddAsync(MessageDto value);
        Task<ServiceResult<Empty>> DeleteAsync(int id);
    }
}
