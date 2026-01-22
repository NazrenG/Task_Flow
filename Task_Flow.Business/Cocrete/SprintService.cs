using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.DTOs;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class SprintService : ISprintService
    {
        private readonly ISprintDal splitDal;
        private readonly ITaskDal taskDal;

        public SprintService(ISprintDal splitDal, ITaskDal taskDal)
        {
            this.splitDal = splitDal;
            this.taskDal = taskDal;
        }

        public async Task AddBacklogToSplit(int workId, int splitId)
        {
            var task = await taskDal.GetById(t => t.Id == workId);
            task.SprintId = splitId;
            await taskDal.Update(task);
        }

        public async Task<Sprint> Create(int projectId, SprintDto splitDto)
        {
            var sprint = new Sprint
            {
                EndDate = splitDto.EndDate,
                StartDate = splitDto.StartDate,
                Name = splitDto.Name,
                ProjectId = projectId,

            };
            await splitDal.Add(sprint);
            return sprint;

        }

        public async Task DeleteSplit(int splitId)
        {
            var split = await splitDal.GetById(p => p.Id == splitId);
            await splitDal.Delete(split);
        }

        public async Task<List<Sprint>> GetSprints(int projectId)
        {
            var sprints = await splitDal.GetAllSprints(projectId);
            return sprints;
        }

        public async Task<Sprint> UpdateTaskSplit(UpdateSprintDto updateSprintDto)
        {
            var task = await taskDal.GetById(t => t.Id == updateSprintDto.TaskId);
            var split = await splitDal.GetSprintById(updateSprintDto.SprintId);

            split.Works.Add(task);
            await splitDal.Update(split);
            return (split);
        }
    }
}