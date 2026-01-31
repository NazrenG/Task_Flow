using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Business.DTOs;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface ICompanyService
    {
        Task CreateCompanyAsync (Company company);
        Task<CompanyDetailDto> GetCompany(string ownerId);
        Task AddWorkerToCompany(CompanyWorker worker,int companyId);
        Task  UserPaidForCompany(string ownerId);
        Task UpdateCompany(int id,string name,string email,string address);
        Task DeleteCompany(int id);
    }
}
