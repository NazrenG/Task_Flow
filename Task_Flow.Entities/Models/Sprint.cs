using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{ 
    public class Sprint : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<Work> Works { get; set; }
    public bool IsOpen { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }
}
}
