using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class GroupChat:IEntity
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string AdminId { get; set; }
        public virtual List<GroupChatMembers>Members { get; set; }
        public virtual List<GroupChatMessage> Messages { get; set; }
        public GroupChat()
        {
            Members = new List<GroupChatMembers>();
        }

    }
}
