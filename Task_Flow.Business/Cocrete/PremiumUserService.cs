using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.ResultServices;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Enums;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class PremiumUserService : IPremiumUserService
    {
        private readonly IUserDal _dal;
        private readonly IProjectDal _project;
        private readonly IRequestNotificationDal _requestNotification;
        private readonly IFriendService _friendService;

        public PremiumUserService(IUserDal userDal, IProjectDal projectDal, IRequestNotificationDal requestNotificationDal, IFriendService friendService)
        {
            _dal = userDal;
            _project = projectDal;
            _requestNotification = requestNotificationDal;
            _friendService = friendService;
        }

        public async Task<ResultService> IsUserAllowedToCreateProjectAsync(string userId, Project project)
        {
            var user = await _dal.GetById(u => u.Id == userId);
            if (user == null) return ResultService.Fail("User not found!");

            if (user.PlanType == PlanType.Free)
                return await _project.GetUserProjectCount(userId) >= 3 ? ResultService.Fail("Free Plan users can't have more than 3 projects!") : (project.EndDate - project.StartDate).TotalDays > 90 ? ResultService.Fail("Free plan users cannot set a project deadline longer than 3 months.") : ResultService.Ok();
            else if (user.PlanType == PlanType.Premium)
                return (project.EndDate - project.StartDate).TotalDays > 366 ? ResultService.Fail("Premium users can set a project deadline up to 1 year. No longer than that.") : ResultService.Ok();

            return ResultService.Ok();
        }

        public async Task<ResultService> IsUserAllowedToSendRequestAsync(string senderId)
        {
            var user = await _dal.GetById(u => u.Id == senderId);
            if (user == null) return ResultService.Fail("User not found!");
            var friends=await _friendService.GetFriends(senderId);

            if (user.PlanType == PlanType.Free)
                if (friends.Count >= 5) ResultService.Fail("Free Plan users cannot send request to more that 5 people.");
            return ResultService.Ok();
        }

        public async Task<ResultService> IsUserAllowedToAddTeammember(int teammemberCount, string userId)
        {
            var user = await _dal.GetById(u => u.Id == userId);
            if (user == null) return ResultService.Fail("User not found!");
            if (user.PlanType == PlanType.Free)
                if (teammemberCount >= 3) return ResultService.Fail("Free users cannot add more than 3 teammembers.");

            return ResultService.Ok();
        }

        public async Task<ResultService> IsUserAllowedToCreateGroupChat(string userId)
        {
            var user=await _dal.GetById(u => u.Id == userId);
            if (user.PlanType == PlanType.Free) return ResultService.Fail("Free Plan users can not create group chat!");

            return ResultService.Ok();
        }

        public async Task UpgradeUserPlanToPremium(string userId)
        {
           var user=await _dal.GetById(u=>u.Id == userId);
            user.PlanType = PlanType.Premium;
        }

        public async Task UpgradeUserPlanToBusiness(string userId)
        {
            var user = await _dal.GetById(u => u.Id == userId);
            user.PlanType = PlanType.Business;
        }
    }
}
