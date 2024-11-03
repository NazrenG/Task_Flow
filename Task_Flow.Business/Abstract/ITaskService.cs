﻿using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Abstract
{
    public interface ITaskService
    {
        Task<List<Work>> GetTasks(string userId);
        Task<Work> GetTaskById(int id);
     
      Task Add(Work task);
      Task Update(Work task);
      Task Delete(Work task);
        Task<List<Work>> GetToDoTask(string userId);
        Task<List<Work>> GetInProgressTask(string userId);
        Task<List<Work>> GetDoneTask(string userId);

    }
}
