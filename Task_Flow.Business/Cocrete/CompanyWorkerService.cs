using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.DTOs;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.DataAccess.Concrete;
using Task_Flow.Entities.Enums;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class CompanyWorkerService : ICompanyWorkerService
    {
        private readonly ICompanyWorkerDal _companyWorkerDal;
        private readonly ICompanyDal _companyDal;
        private readonly IUserDal _userDal;
        public CompanyWorkerService(ICompanyWorkerDal companyWorkerDal, IUserDal userDal, ICompanyDal companyDal)
        {
            _companyWorkerDal = companyWorkerDal;
            _userDal = userDal;
            _companyDal = companyDal;
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
            worker.IsDeleted = true;
            await _companyWorkerDal.Update(worker);
        }

        public async Task<List<SearchedWorkerDto>> SearchWorkerByKey(string key, int companyId)
        {
            key = key?.Trim().ToLower();
            var workers=await _companyWorkerDal.GetAll(c=>c.CompanyId==companyId);
            if (!workers.Any())
                return new List<SearchedWorkerDto>();

            var userIds = workers.Select(w => w.UserId).ToList();

            var users = await _userDal.GetAll(u =>
       userIds.Contains(u.Id) &&
       u.UserName.ToLower().Contains(key)
   );

            var result = workers
      .Where(w => users.Any(u => u.Id == w.UserId))
      .Select(w =>
      {
          var user = users.First(u => u.Id == w.UserId);
          return new SearchedWorkerDto
          {
              Username = user.UserName,
          };
      })
      .ToList();

            return result;
        }

        public async Task<List<string>> CompanyReopened(int companyId)
        {
            var workers = await _companyWorkerDal.GetAll(cw => cw.CompanyId == companyId);
            if (!workers.Any())
                return new List<string>();
            foreach (var worker in workers)
            {
                worker.IsRemoved = false;
                await _companyWorkerDal.Update(worker);
            }
            var userIds = workers.Select(w => w.UserId).Distinct().ToList();
            var users = await _userDal.GetAll(u => userIds.Contains(u.Id));
            foreach (var user in users)
            {
                user.PlanType = PlanType.CompanyWorker;
                await _userDal.Update(user);
            }
            return userIds;
        }

        public async Task<List<string>> CompanyDeleted(int companyId)
        {
            var workers = await _companyWorkerDal.GetAll(cw => cw.CompanyId == companyId);
            if (!workers.Any())
                return new List<string>();
            foreach (var worker in workers)
            {
                worker.IsRemoved = true;
            await _companyWorkerDal.Update(worker);
            }
            var userIds = workers.Select(w => w.UserId).Distinct().ToList();

            var users = await _userDal.GetAll(u => userIds.Contains(u.Id));
            foreach (var user in users)
            {
                user.PlanType= 0;
            await _userDal.Update(user);
            }
            return userIds;
        }

        public async Task<List<SelectedCompanyUserDto>> SelectedUsersForCompany(int companyId)

        {
            var firstlistUserIds=(await _companyWorkerDal.GetAll()).Select(u=>u.UserId);
            var company=await _companyDal.GetById(c=>c.Id==companyId);
            var usersNotInSecondList = (await _userDal.GetAll())
            .Where(u => !firstlistUserIds.Contains(u.Id)&&u.Id!=company.OwnerId && u.PlanType!= PlanType.Business)
            .ToList();

            return  usersNotInSecondList.Select(u=>new SelectedCompanyUserDto { Fullname=u.Firstname+" "+u.Lastname,Username=u.UserName, Id=u.Id}).ToList();

        }
    }
}
