using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Business.ResultServices;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
    public interface IPremiumUserService
    {
        Task<ResultService> IsUserAllowedToCreateProjectAsync(string userId, Project project);
        Task<ResultService> IsUserAllowedToSendRequestAsync(string senderId);
        Task<ResultService> IsUserAllowedToAddTeammember(int teammemberCount, string userId);
        Task<ResultService>IsUserAllowedToCreateGroupChat(string userId);
        Task UpgradeUserPlanToPremium(string userId);
        Task UpgradeUserPlanToBusiness(string userId);

    }
}
