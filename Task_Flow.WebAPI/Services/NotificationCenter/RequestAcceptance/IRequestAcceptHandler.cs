using Task_Flow.Entities.Models;

namespace Task_Flow.WebAPI.Services.NotificationCenter.RequestAcceptance
{
    /// <summary>
    /// Müəyyən tipli sorğu qəbul edildikdə görüləcək iş.
    /// Yeni sorğu tipi üçün mövcud kodu dəyişmədən yeni handler əlavə etmək kifayətdir (Open/Closed).
    /// </summary>
    public interface IRequestAcceptHandler
    {
        string NotificationType { get; }
        Task HandleAsync(RequestNotification request, string userId);
    }
}
