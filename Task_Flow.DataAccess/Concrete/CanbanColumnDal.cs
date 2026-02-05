using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using TaskFlow.Core.DataAccess.EntityFramework;

namespace Task_Flow.DataAccess.Concrete
{
    public class CanbanColumnDal : EFEntityBaseRepository<TaskFlowDbContext, CanbanColumn>, ICanbanColumnDal
    {
        private readonly TaskFlowDbContext taskFlowDbContext;
        public CanbanColumnDal(TaskFlowDbContext context) : base(context)
        {
            taskFlowDbContext = context;
        }

        public async Task<List<CanbanColumn>> GetAllColumn(int sprintId)
        {
            return await taskFlowDbContext.CanbanColumns.Where(c => c.SprintId == sprintId).Include(w => w.TaskForUsers).OrderBy(c => c.Order).ToListAsync();
        }
    }
}
