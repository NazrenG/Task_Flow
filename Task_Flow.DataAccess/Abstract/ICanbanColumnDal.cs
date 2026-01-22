using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Core.DataAccess;
using Task_Flow.Entities.Models;

namespace Task_Flow.DataAccess.Abstract
{
    public interface ICanbanColumnDal : IEntityRepository<CanbanColumn>
    {
        public Task<List<CanbanColumn>> GetAllColumn(int sprintId);
    }
}
