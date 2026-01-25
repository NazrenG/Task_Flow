using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class UserTaskService:IUserTaskService
    {
        private readonly IUserTaskDal taskAssignDal;

        public UserTaskService(IUserTaskDal taskAssignDal)
        {
            this.taskAssignDal = taskAssignDal;
        }

        public async Task Add(UserTask assign)
        {
           var list=await taskAssignDal.GetAll();
            var items= list.FirstOrDefault(p=>p.Title == assign.Title);
            if (items==null) await taskAssignDal.Add(assign);   

        }

        public async Task<bool> CheckTaskName(string title)
        {

            var list = await taskAssignDal.GetAll();
            return list.Any(p=>p.Title == title);   //varsa-true
        }

        public async Task Delete(UserTask assign)
        {
            var task=await taskAssignDal.GetById(t=>t.Id==assign.Id);
            task.IsDeleted = true;
            await taskAssignDal.Update(assign);
        }

        public async Task<UserTask> GetById(int id)
        {
            return await taskAssignDal.GetById(t=>t.Id==id);
        }

        public async Task<List<UserTask>> GetDoneTask(string userId)
        {
            return await taskAssignDal.GetAll(t => t.Status!.ToLower() == "done" && t.CreatedById == userId);
        }

        public async Task<List<UserTask>> GetInProgressTask(string userId)
        {
            return await taskAssignDal.GetAll(t => t.Status!.ToLower() == "in progress" && t.CreatedById == userId);
        }

        public async Task<List<UserTask>> GetToDoTask(string userId)
        {
            return await taskAssignDal.GetAll(t => t.Status!.ToLower() == "to do" && t.CreatedById == userId);
        }

        public async Task<List<UserTask>> GetUserTasks(string userId)
        {
            return await taskAssignDal.GetAll(u=>u.CreatedById==userId);
        }

        public async Task Update(UserTask assign)
        {
            await taskAssignDal.Update(assign); 
        }
    }
}
