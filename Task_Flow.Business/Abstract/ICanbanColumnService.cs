using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Business.DTOs;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface ICanbanColumnService
    {
        Task CreateCanbanColumn(CreateCanbanDto createCanbanDto);
        Task<List<CanbanColumn>> GetAllCanbanColumn(int sprintId);
        Task DeleteCanbanColumn(int canbanColumnId);
        Task CreateDefaultCanbanName(CreateDefaultCanbanNameDto createDefaultCanbanNameDto);
    }
}
