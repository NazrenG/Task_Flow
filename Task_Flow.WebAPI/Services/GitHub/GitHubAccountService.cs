using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.GitHub
{
    public class GitHubAccountService : IGitHubAccountService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public GitHubAccountService(IGitHubService gitHubService, IUserService userService, IConfiguration configuration)
        {
            _gitHubService = gitHubService;
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<ServiceResult<object>> GetAuthorizationUrlAsync(string userId)
        {
            var authUrl = await _gitHubService.GetAuthorizationUrl(userId);
            return ServiceResult<object>.Success(new { authUrl });
        }

        // userId GitHub-dan "state" parametri kimi geri qayıdır
        public async Task ConnectAccountAsync(string code, string userId)
        {
            var accessToken = await _gitHubService.ExchangeCodeForToken(code, userId);
            var githubUsername = await _gitHubService.GetUsername(accessToken);

            var user = await _userService.GetUserById(userId);
            if (user != null)
            {
                user.GitHubAccessToken = accessToken;
                user.GitHubUsername = githubUsername;
                await _userService.Update(user);
            }
        }

        public string GetProfileRedirectUrl(bool success)
        {
            var result = success ? "success" : "error";
            return $"{_configuration["FrontendUrl"]}/profile?github={result}";
        }
    }
}
