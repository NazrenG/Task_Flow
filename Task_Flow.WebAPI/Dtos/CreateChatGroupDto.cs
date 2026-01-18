using Task_Flow.Entities.Models;

namespace Task_Flow.WebAPI.Dtos
{
    public class CreateChatGroupDto
    {
     
        public string GroupName { get; set; }
        public List<string> Members{get;set;}

    }
}
