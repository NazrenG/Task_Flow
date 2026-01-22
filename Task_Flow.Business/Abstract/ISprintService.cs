using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Business.DTOs;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface ISprintService
    {
        public Task<Sprint> Create(int projectId, SprintDto splitDto);
        public Task AddBacklogToSplit(int workId, int splitId);
        public Task DeleteSplit(int splitId);
        public Task<List<Sprint>> GetSprints(int projectId);
        public Task<Sprint> UpdateTaskSplit(UpdateSprintDto updateSprintDto);
        // public Task<List<>>
    }
}
