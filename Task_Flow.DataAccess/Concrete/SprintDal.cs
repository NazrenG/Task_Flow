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
    public class SprintDal : EFEntityBaseRepository<TaskFlowDbContext, Sprint>, ISprintDal
    {
        public TaskFlowDbContext dbContext;
        public SprintDal(TaskFlowDbContext context) : base(context)
        {
            dbContext = context;
        }

        public async Task<List<Sprint>> GetAllSprints(int projectId)
        {
            return await dbContext.Sprints.Include(w => w.Works).Where(p => p.ProjectId == projectId).ToListAsync();
        }

        public async Task<Sprint> GetSprintById(int id)
        {
            return dbContext.Sprints.Include(s => s.Works).FirstOrDefault(t => t.Id == id);
        }
    }
}
