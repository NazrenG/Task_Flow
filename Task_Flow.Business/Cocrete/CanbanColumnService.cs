using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    public class CanbanColumnService : ICanbanColumnService
    {
        private readonly ICanbanColumnDal kanbanColumnDal;
        private readonly IWorkDal workDal;



        public CanbanColumnService(ICanbanColumnDal kanbanColumnDal, IWorkDal workDal)
        {
            this.kanbanColumnDal = kanbanColumnDal;
            this.workDal = workDal;
        }

        public async Task CreateCanbanColumn(CreateCanbanDto createCanbanDto)
        {
            var allColumn = await kanbanColumnDal.GetAll();
            var afterColumn = allColumn.FirstOrDefault(x => x.Id == createCanbanDto.InsertAfterColumnId);

            if (afterColumn == null) throw new Exception("Reference column not found");

            var columnsToShift = allColumn
     .Where(x =>
         x.ProjectId == createCanbanDto.ProjectId &&
         x.Order > afterColumn.Order)
     .ToList();

            foreach (var col in columnsToShift)
                col.Order++;

            var newColumn = new CanbanColumn
            {
                ProjectId = createCanbanDto.ProjectId,
                Name = createCanbanDto.Title,
                StatusKey = createCanbanDto.Title.ToLower(),
                Order = afterColumn.Order + 1,
                IsFixed = false
            };

            await kanbanColumnDal.Add(newColumn);
        }

        public async Task CreateDefaultCanbanName(CreateDefaultCanbanNameDto createDefaultCanbanNameDto)
        {
            await kanbanColumnDal.Add(new CanbanColumn
            {
                Name = createDefaultCanbanNameDto.Name,
                ProjectId = createDefaultCanbanNameDto.ProjectId,
                StatusKey = createDefaultCanbanNameDto.StatusKey,
                Order = createDefaultCanbanNameDto.Order,
                IsFixed = true


            });
        }

        public async Task DeleteCanbanColumn(int canbanNameId)
        {
            var canbanName = await kanbanColumnDal.GetById(p => p.Id == canbanNameId);

            if (canbanName == null)
                throw new Exception("Column not found");

            if (canbanName.IsFixed)
                throw new Exception("Fixed columns cannot be deleted");

            var allCanbanColumn = await kanbanColumnDal.GetAll();
            var todoColumn = allCanbanColumn.First(x =>
                 x.ProjectId == canbanName.ProjectId &&
                 x.StatusKey == "todo");
            if (todoColumn == null) throw new Exception("To Do column not found");

            var tasks = await workDal.GetAll(t => t.CanbanColumnId == canbanNameId)
       ;

            foreach (var task in tasks)
                task.CanbanColumnId = todoColumn.Id;

            await kanbanColumnDal.Delete(canbanName);

            var columnsToFix = await kanbanColumnDal.GetAll(x =>
          x.ProjectId == canbanName.ProjectId &&
          x.Order > canbanName.Order);

            foreach (var col in columnsToFix)
                col.Order--;

            // await kanbanColumnDal.Update(canbanName);
        }



        //public async Task<List<CanbanColumn>> GetAllCanbanColumn(int projectId)
        //{
        //  var list= await kanbanColumnDal.GetAllColumn(projectId);
        //    return list;
        //        }

        public async Task<List<CanbanColumn>> GetAllCanbanColumn(int projectId)
        {
            var list = await kanbanColumnDal.GetAllColumn(projectId);
            return list;
        }
    }
}