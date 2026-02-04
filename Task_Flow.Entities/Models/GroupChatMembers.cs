using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;
using Task_Flow.Entities.Enums;

namespace Task_Flow.Entities.Models
{
    public class GroupChatMembers:IEntity
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string UserId { get; set; }
        public bool IsRemoved { get; set; }
        public GroupRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual CustomUser CustomUser { get; set; }
        public virtual GroupChat GroupChat { get; set; }

    }
}
