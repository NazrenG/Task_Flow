using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Flow.Business.ResultServices
{
    public class ResultService
    {
        public bool Allowed { get; set; }
        public string Message { get; set; }

        public static ResultService Ok() =>
            new ResultService { Allowed = true, Message = "Allowed" };

        public static ResultService Fail(string message) =>
            new ResultService { Allowed = false, Message = message };
    }
}
