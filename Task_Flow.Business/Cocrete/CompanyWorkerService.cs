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
    public class CompanyWorkerService : ICompanyWorkerService
    {
        private readonly ICompanyWorkerDal _companyWorkerDal;
        public CompanyWorkerService(ICompanyWorkerDal companyWorkerDal)
        {
            _companyWorkerDal = companyWorkerDal;
        }

        public async Task AddWorkerToCompany(CompanyWorker companyWorker)
        {
            await _companyWorkerDal.Add(companyWorker);
        }

        public async Task<List<CompanyWorker>> GetAllCompanyWorkers(System.Linq.Expressions.Expression<Func<CompanyWorker, bool>> predicate = null)
        {
            return await _companyWorkerDal.GetAll(predicate); 
        }

        public async Task<CompanyWorker> GetCompanyWorkers(int workerId)
        {
            return await _companyWorkerDal.GetById(w=>w.Id==workerId);
        }

        public async Task RemoveCompanyWorker(int workerId)
        {
            var worker = await _companyWorkerDal.GetById(w=>w.Id==workerId);
            await _companyWorkerDal.Delete(worker);
        }
    }
}
