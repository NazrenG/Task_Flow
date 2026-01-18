namespace Task_Flow.WebAPI.Dtos
{
    public class GroupChatMessageDto
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public bool IsSender { get; set; }
        public string Message { get; set; }
        public string Photo {  get; set; }
        public string SentDate { get; set; }
    }
}
