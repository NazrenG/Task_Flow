using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class CompanyWorker:IEntity
    {
        public int Id { get; set; }
        public string UserId{ get; set; }
        public int CompanyId { get; set; }
        public DateTime JoinedDate {  get; set; }
        public string Occupation {  get; set; }
        public int Role {  get; set; }
        public bool IsDeleted { get; set; }
        public virtual Company Company { get; set; }
        public virtual CustomUser CustomUser { get; set; }
    
    
    }
}
