using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Flow.Business.Abstract
{
    public interface IGitHubService
    {
        Task<string> GetAuthorizationUrl(string userId);
        Task<string> ExchangeCodeForToken(string code, string state);
        Task<string> CreateRepository(string accessToken, string repoName, string description);
        Task<bool> AddCollaborator(string accessToken, string repoOwner, string repoName, string collaboratorUsername);
        Task<bool> CreateBranch(string accessToken, string repoOwner, string repoName, string branchName, string fromBranch = "main");
    }
}
