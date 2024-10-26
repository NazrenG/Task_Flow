﻿using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Abstract
{
    public interface IProjectService
    {

        Task<List<Project>> GetProjects();
        Task<Project> GetProjectById(int id);
        Task Add(Project project);
        Task Update(Project project);
        Task Delete(Project project);
        Task<int> GetUserProjectCount(string userId);
    }
}
