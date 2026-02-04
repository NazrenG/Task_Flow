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
        private readonly ICanbanColumnDal canbanColumnDal;

        public SprintService(ISprintDal splitDal, ITaskDal taskDal, ICanbanColumnDal canbanColumnDal)
        {
            this.splitDal = splitDal;
            this.taskDal = taskDal;
            this.canbanColumnDal = canbanColumnDal;
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
            var sprint = await splitDal.GetSprintById(updateSprintDto.SprintId);




            //project kanban elaqesine gore deafult to do ya elave et , sonradan deyis sprint colummn elaqesine gore

            var canbanColumn = await canbanColumnDal.GetById(s => s.SprintId == sprint.Id && s.StatusKey == "to do");
            task.CanbanColumnId =canbanColumn.Id;



            await taskDal.Update(task);

            sprint.Works.Add(task);
            await splitDal.Update(sprint);
            return (sprint);
        }
    }
}