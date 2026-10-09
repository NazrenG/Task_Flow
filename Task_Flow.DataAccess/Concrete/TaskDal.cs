using Microsoft.EntityFrameworkCore;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using TaskFlow.Core.DataAccess.EntityFramework;

namespace Task_Flow.DataAccess.Concrete
{
    public class TaskDal : EFEntityBaseRepository<TaskFlowDbContext, Work>, ITaskDal
    {

        private readonly TaskFlowDbContext _context;
        public TaskDal(TaskFlowDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Work>> GetAllTask()
        {
            return await _context.Works.Include(p => p.Project).Include(w => w.CreatedBy).Include(t=>t.TaskAssignees).ToListAsync();

        }

        public async Task<List<Work>> GetTasksByProjectOwner(string ownerId)
        {
            return await _context.Works
                .Include(w => w.Project)
                .Include(w => w.CreatedBy)
                .Where(w => w.Project.CreatedById == ownerId)
                .ToListAsync();
        }

        public async Task<List<int>> GetTaskSummaryByMonthAsync(int projectId, int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var tasks = await _context.Works
                .Where(work => work.ProjectId == projectId
                               && work.StartTime >= startDate
                               && work.StartTime < endDate)
                .ToListAsync();


            var list = new List<int>();
            int completedCount = tasks.Count(t => t.Status == "Completed");
            int ongoingCount = tasks.Count(t => t.Status == "On Going");
            list.Add(completedCount);
            list.Add(ongoingCount);

            return list;
        }
    }
}
