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
        private readonly IProjectDal _projectDal;
        public CompanyService(ICompanyDal companyDal, IProjectDal projectDal)
        {
            _companyDal = companyDal;
            _projectDal = projectDal;
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
            (await _projectDal.GetAll(p => p.CompanyId == company.Id)).Select(p => p.IsDeleted = true);
            await _companyDal.Update(company);
        }

        public async Task<CompanyDetailDto> GetCompany(string ownerid)
        {
            var company= await _companyDal.GetById(c => c.OwnerId == ownerid);
            return company!=null?new CompanyDetailDto
            {
                CompanyId = company.Id,
                CompanyName = company.Name,
                IsPaid = company.IsPaid,
                IsDeleted =company.IsDeleted,
                Address = company.Address,
                Email = company.Email,
                CreatedDate = company.CreatedDate.ToShortTimeString(),
            }:null;
        }

        public async Task<List<CompanyProjectDto>> GetCompanyProjects(int companyId)
        {
            var projects = await _projectDal.GetAll(c => c.CompanyId==companyId);
            var list =projects.Select(p => { return new CompanyProjectDto { Id = p.Id, ProjectName = p.Title }; }).ToList();
            return list;
        }

     

        public async Task UpdateCompany(int id, string name, string email, string address)
        {
            var company = await _companyDal.GetById(c=>c.Id==id);
            company.Name = name;
            company.Email = email;
            company.Address = address;
            await _companyDal.Update(company);
        }

        public async Task<bool>IsCompanyProject(int projectId)
        {
           var project=await _projectDal.GetById(p=>p.Id==projectId);
            return project!=null;


        }



        public async Task UserPaidForCompany(string ownerId)
        {
           var company =await _companyDal.GetById(c=>c.OwnerId == ownerId);
            company.IsPaid = true;
            company.IsDeleted = false;
            await _companyDal.Update(company);
        }
    } 
}
