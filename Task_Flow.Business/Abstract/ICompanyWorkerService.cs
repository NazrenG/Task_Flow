using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface ICompanyWorkerService
    {
        Task<List<CompanyWorker>> GetAllCompanyWorkers(Expression<Func<CompanyWorker, bool>> predicate = null);
        Task<CompanyWorker> GetCompanyWorkers(int workerId);
        Task RemoveCompanyWorker(int workerId);
        Task AddWorkerToCompany(CompanyWorker companyWorker);
        Task SelectedUsersForCompany(int companyId);
    }
}
