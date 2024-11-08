﻿using Task_Flow.Core.DataAccess;
using Task_Flow.Entities.Models;


namespace Task_Flow.DataAccess.Abstract
{
    public interface IProjectDal : IEntityRepository<Project>
    {
       public Task<List<Project>> GetAllProjects();
    }
}
