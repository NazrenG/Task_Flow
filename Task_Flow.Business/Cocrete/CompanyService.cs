using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyDal _companyDal;
        public CompanyService(ICompanyDal companyDal)
        {
            _companyDal = companyDal;
        }

        public async Task AddWorkerToCompany(CompanyWorker worker, int companyId)
        {
            var company=await _companyDal.GetById(c=>c.Id==companyId);
            //company
        }

        public async Task CreateCompanyAsync(Company company)
        {
             await _companyDal.Add(company);
        }

        public Task<Company> GetCompany(int companyId)
        {
            return _companyDal.GetById(c=>c.Id== companyId);
        }
    }
}
