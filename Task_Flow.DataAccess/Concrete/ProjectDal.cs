﻿using Microsoft.EntityFrameworkCore;
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
            return await _db.Projects.Include(p => p.TaskForUsers) 
                   .Include(o => o.TeamMembers)  
                       .ThenInclude(tm => tm.User).ToListAsync();
        }

        public async Task<Project> GetProjectById(int projectId)
        {
           return   _db.Projects.Include(p => p.TaskForUsers)
                   .Include(o => o.TeamMembers)
                       .ThenInclude(tm => tm.User).FirstOrDefault(p=>p.Id==projectId);
        }
    }
}
