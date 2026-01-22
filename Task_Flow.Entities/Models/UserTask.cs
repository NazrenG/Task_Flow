using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
   public class UserTask:IEntity
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime Deadline { get; set; }
        public string? Status { get; set; }//to do,in progress,done

        public string? Priority { get; set; }// Urgent, Primary, Simple

        public string? Color { get; set; }
        public string? CreatedById { get; set; }//userId
        public bool IsDeleted { get; set; } = false;
        public virtual CustomUser? CreatedBy { get; set; }
    }
}
