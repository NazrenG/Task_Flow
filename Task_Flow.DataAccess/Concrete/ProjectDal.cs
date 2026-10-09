using Microsoft.EntityFrameworkCore;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using TaskFlow.Core.DataAccess.EntityFramework;

namespace Task_Flow.DataAccess.Concrete
{
    public class ProjectDal : EFEntityBaseRepository<TaskFlowDbContext, Project>, IProjectDal
    {
        private readonly TaskFlowDbContext _db; 
             
        public ProjectDal(TaskFlowDbContext context) : base(context)
        {
            _db = context;  
        }

        public async Task<List<Project>> GetAllProjects()
        {
            return await _db.Projects.Include(p => p.TaskForUsers) .Include(u => u.CreatedBy)
                   .Include(o => o.TeamMembers)  
                       .ThenInclude(tm => tm.User).ToListAsync();
        }

        public async Task<Project> GetProjectById(int projectId)
        {
           return   _db.Projects.Include(u=>u.CreatedBy).Include(p => p.TaskForUsers)
                   .Include(o => o.TeamMembers)
                       .ThenInclude(tm => tm.User).FirstOrDefault(p=>p.Id==projectId);
        }
        public async Task<Project?> GetProjectWithTasksById(int projectId)
        {
            return await _db.Projects
                .Include(p => p.TaskForUsers)
                .FirstOrDefaultAsync(p => p.Id == projectId);
        }

        public async Task<int> GetUserProjectCount(string userId)
        {
            return _db.Projects.Count(u => u.CreatedById == userId);
        }
    }
}
