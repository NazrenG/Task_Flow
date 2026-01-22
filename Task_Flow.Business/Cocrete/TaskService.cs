using Microsoft.EntityFrameworkCore;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class TaskService : ITaskService
    {
        private readonly ITaskDal dal;

        public TaskService(ITaskDal dal)
        {
            this.dal = dal;
        }

        public async Task Add(Work task)
        {
            await dal.Add(task);
        }

        public async Task Delete(Work task)
        {
            await dal.Delete(task);
        }

        public async Task<List<Work>> GetByProjectId(int projectId)
        {
            var list = await dal.GetAllTask();
            return list.Where(t => t.ProjectId == projectId && t.SprintId != null).ToList();
        }

        public async Task<List<Work>> GetDoneTask(string userId)
        {
                return await dal.GetAll(t => t.Status!.ToLower() == "done" && t.CreatedById==userId);
        }

        public async Task<List<Work>> GetInProgressTask(string userId)
        {
            return await dal.GetAll(t => t.Status!.ToLower() == "in progress" && t.CreatedById == userId);
        }

        public async Task<Work> GetTaskById(int id)
        {
            return await dal.GetById(f => f.Id == id);
        }

        public async Task<List<Work>> GetTasks(string userId)
        {
            var list = await dal.GetAllTask();
            return  list.Where(p=>p.CreatedById == userId).ToList();   
        }

        public async Task<List<Work>> GetToDoTask(string userId)
        {
            return await dal.GetAll(t => t.Status!.ToLower() == "to do" && t.CreatedById == userId);
        }

        public async Task<List<int>> GetTaskSummaryByMonthAsync(int projectId,int month,int year)
        {
          return await dal.GetTaskSummaryByMonthAsync(projectId,month,year);
        }

        public async Task Update(Work task)
        {
            await dal.Update(task);
        }

        public async Task<List<Work>> GetBacklogs(int projectId)
        {
            var list = await dal.GetAllTask();
            return list.Where(p => p.ProjectId == projectId && p.SprintId == null).ToList();
        }
    }
}
