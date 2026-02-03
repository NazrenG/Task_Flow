
namespace Task_Flow.WebAPI
{
    public class RequestNotificationDto
    {
        public string? Text { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverEmail { get; set; }
        public bool IsAccepted { get; set; }
        
        public string? NotificationType { get; set; }

    }
}