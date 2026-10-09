using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Abstract
{
    public interface IProjectService
    {

        Task<List<Project>> GetProjects(string userId);
        Task<Project> GetProjectById(int id);
        Task<Project?> GetProjectWithTasksById(int id);
        Task<string> GetProjectNameById(int id);
        Task Add(Project project);
        Task Update(Project project);
        Task Delete(Project project);
        Task<int> GetUserProjectCount(string userId);
        Task<List<Project>> GetOnGoingProject(string userId);
        Task<List<Project>> GetPendingProject(string userId);
        Task<List<Project>> GetCompletedTask(string userId);
        //Task<string> GetProjectNameById(int projecId);
        Task<Project> GetProjectByName(string userId, string projectName);
    }
}
