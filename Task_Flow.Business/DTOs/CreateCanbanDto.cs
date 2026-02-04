using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Flow.Business.DTOs
{
    public class CreateCanbanDto
    {
        public int SprintId { get; set; }
        public string Title { get; set; } = null!;
        public int InsertAfterColumnId { get; set; }
    }
}
