using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface ICompanyService
    {
        Task CreateCompanyAsync (Company company);
        Task<Company> GetCompany(int companyId);
        Task AddWorkerToCompany(CompanyWorker worker,int companyId);
    }
}
