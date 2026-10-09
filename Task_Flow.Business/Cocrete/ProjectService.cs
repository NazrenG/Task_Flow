using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectDal dal;

        public ProjectService(IProjectDal dal)
        {
            this.dal = dal;
        }

        public async Task Add(Project project)
        {
            var list = await dal.GetAll();
            var check = list.FirstOrDefault(p => p.Title == project.Title);
            if (check == null)
                await dal.Add(project);
        }

        public async Task Delete(Project project)
        {
            await dal.Delete(project);
        }

        public async Task<Project> GetProjectById(int id)
        {
            return await dal.GetProjectById(id);
        }

        public async Task<Project?> GetProjectWithTasksById(int id)
        {
            return await dal.GetProjectWithTasksById(id);
        }

        public async Task<List<Project>> GetProjects(string userId)
        {
            var items = await dal.GetAllProjects();

            return items.Where(p => p.CreatedById == userId).ToList();
        }

       

        public async Task Update(Project project)
        {
            await dal.Update(project);
        }

        public async Task<int> GetUserProjectCount(string userId)
        {
            var items = await dal.GetAllProjects();

            return items.Where(p => p.CreatedById == userId).ToList().Count;

        }

        public async Task<List<Project>> GetOnGoingProject(string userId)
        {
            var items = await dal.GetAllProjects();
            return   items.Where(u => u.CreatedById == userId && u.Status!.ToLower() == "on going").ToList();
           
        }

        public async Task<List<Project>> GetPendingProject(string userId)
        {
            return await dal.GetAll(u => u.CreatedById == userId && u.Status!.ToLower() == "pending");

        }

        public async Task<List<Project>> GetCompletedTask(string userId)
        {
            return await dal.GetAll(u => u.CreatedById == userId && u.Status!.ToLower() == "completed");

        }

        public async Task<Project> GetProjectByName(string userId, string projectName)
        {
            return await dal.GetById(u => u.CreatedById == userId && u.Title.ToLower() == projectName.ToLower());

        }

        public async Task<string> GetProjectNameById(int id)
        {
            var item= await dal.GetProjectById(id);
            return item.Title;
        }

        //public async Task<string> GetProjectByName(int projecId)
        //{
        //    var name= await dal.GetById(p => p.Id == projecId);
        //    if (name.Title == null)
        //    {
        //        throw new InvalidOperationException("Project not found.");
        //    }
        //    return name.Title;
        //}
    }
}
