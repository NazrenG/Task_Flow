using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class ChatMessage:IEntity
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public bool IsImage { get; set; }
        public DateTime SentDate { get; set; }
        public int ChatId { get; set; }
        public string? IV { get; set; }

        public virtual Chat? Chat { get; set; }
        public bool HasSeen { get; set; }
        public string? SenderId { get; set; }
        public virtual CustomUser? Sender { get; set; }
        public string Status { get; set; } = "Sent";
        public ChatMessage()
        {
            SentDate = DateTime.Now;
        }
    }
}
