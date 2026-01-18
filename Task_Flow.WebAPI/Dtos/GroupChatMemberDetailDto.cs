namespace Task_Flow.WebAPI.Dtos
{
    public class GroupChatMemberDetailDto
    {
        public string UserId { get; set; } 
        public string Fullname { get; set; }
        public string ProfileImage  { get; set; }
        public bool IsAdmin { get; set; }
        public string JoinedAt { get; set; }
    }
}
