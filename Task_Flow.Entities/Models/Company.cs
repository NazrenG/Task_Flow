using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class Company:IEntity
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Address { get; set; }
        public string Email {  get; set; }
        List<CompanyWorker> Workers{ get; set; }
        public Company()
        {
            Workers = new List<CompanyWorker>();
        }
    }
}
