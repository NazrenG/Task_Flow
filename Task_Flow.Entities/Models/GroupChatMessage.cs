using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class GroupChatMessage:IEntity
    {
        public int Id { get; set; }

        public int GroupChatId { get; set; }

        public string SenderId { get; set; }

        public string Content { get; set; }
        public string IV { get; set; } 

        public bool IsDeleted{ get; set; } 
        public DateTime SentDate { get; set; }
        public virtual CustomUser Sender { get; set; }
        public virtual GroupChat GroupChat { get; set; }
    }
}
