using Task_Flow.Entities.Models;

namespace Task_Flow.WebAPI.Dtos
{
    public class GroupChatDetailsDto
    {
        public string GroupName { get; set; }
        public string CreatedDate { get; set; }
        public List<GroupChatMemberDetailDto>GroupChatMembers { get; set; }
    }
}
