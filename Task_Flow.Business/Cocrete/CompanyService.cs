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

        public async Task DeleteCompany(int id)
        {
          var company=await _companyDal.GetById(c=>c.Id == id);
            company.IsDeleted = true;
            await _companyDal.Update(company);
        }

        public async Task<CompanyDetailDto> GetCompany(string ownerid)
        {
            var company= await _companyDal.GetById(c => c.OwnerId == ownerid);
            return company!=null?new CompanyDetailDto
            {
                CompanyId = company.Id,
                CompanyName = company.Name,
                İsPaid = company.IsPaid,
                Address = company.Address,
                Email = company.Email,
                CreatedDate = company.CreatedDate.ToShortTimeString(),
            }:null;
        }

        public async Task UpdateCompany(int id, string name, string email, string address)
        {
            var company = await _companyDal.GetById(c=>c.Id==id);
            company.Name = name;
            company.Email = email;
            company.Address = address;
            await _companyDal.Update(company);
        }

        public async Task UserPaidForCompany(string ownerId)
        {
           var company =await _companyDal.GetById(c=>c.OwnerId == ownerId);
            company.IsPaid = true;
            await _companyDal.Update(company);
        }
    }
}
