using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Flow.Business.DTOs
{
    public class CreateDefaultCanbanNameDto
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public int ProjectId { get; set; }
        public string StatusKey { get; set; }
    }
}
