using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Friends
{
    /// <summary>
    /// Dost əlavə etmək, dostluqdan çıxmaq və sorğunu silmək.
    /// </summary>
    public interface IFriendCommandService
    {
        Task<ServiceResult<Friend>> FollowAsync(string userId, SendFollowFriendDto value);
        Task<ServiceResult<object>> UnfollowAsync(string userId, string friendEmail);
        Task<ServiceResult<Empty>> DeleteRequestAsync(int requestId);
    }
}
