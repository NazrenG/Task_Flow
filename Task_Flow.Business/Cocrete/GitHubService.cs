using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.DTOs;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Cocrete
{
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _httpClient;
        private readonly GitHubSettings _settings;
        private readonly IConfiguration _configuration;

        public GitHubService(HttpClient httpClient, IOptions<GitHubSettings> settings, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _configuration = configuration;

            _httpClient.DefaultRequestHeaders.Add("User-Agent", _settings.AppName);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
        }

        public Task<string> GetAuthorizationUrl(string userId)
        {
            var redirectUri = $"{_configuration["AppUrl"]}/api/GitHub/callback";
            var scope = "repo,write:org,admin:org"; // Repository və organization permissions

            var url = $"https://github.com/login/oauth/authorize?" +
                      $"client_id={_settings.ClientId}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                      $"&scope={Uri.EscapeDataString(scope)}" +
                      $"&state={userId}"; 

            return Task.FromResult(url);
        }

        public async Task<string> ExchangeCodeForToken(string code, string state)
        {
            var tokenRequest = new
            {
                client_id = _settings.ClientId,
                client_secret = _settings.ClientSecret,
                code = code,
                redirect_uri = $"{_configuration["AppUrl"]}/api/GitHub/callback"
            };

            var response = await _httpClient.PostAsJsonAsync(
                "https://github.com/login/oauth/access_token",
                tokenRequest
            );

            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<GitHubTokenResponseDto>();
            return tokenResponse.AccessToken;
        }

        public async Task<string> CreateRepository(string accessToken, string repoName, string description)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var createRepoRequest = new
            {
                name = repoName,
                description = description,
                @private = true, // Private repository
                auto_init = true // README elave olunsun
            };

            var response = await _httpClient.PostAsJsonAsync(
                "https://api.github.com/user/repos",
                createRepoRequest
            );

            response.EnsureSuccessStatusCode();

            var repo = await response.Content.ReadFromJsonAsync<JsonElement>();
            return repo.GetProperty("html_url").GetString();
        }

        public async Task<bool> AddCollaborator(string accessToken, string repoOwner, string repoName, string collaboratorUsername)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var addCollaboratorRequest = new
            {
                permission = "push" 
            };

            var response = await _httpClient.PutAsJsonAsync(
                $"https://api.github.com/repos/{repoOwner}/{repoName}/collaborators/{collaboratorUsername}",
                addCollaboratorRequest
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateBranch(string accessToken, string repoOwner, string repoName, string branchName, string fromBranch = "main")
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            // 1. Get reference SHA from main/master branch
            var refResponse = await _httpClient.GetAsync(
                $"https://api.github.com/repos/{repoOwner}/{repoName}/git/refs/heads/{fromBranch}"
            );

            if (!refResponse.IsSuccessStatusCode)
                return false;

            var refData = await refResponse.Content.ReadFromJsonAsync<JsonElement>();
            var sha = refData.GetProperty("object").GetProperty("sha").GetString();

            // 2. Create new branch
            var createBranchRequest = new
            {
                @ref = $"refs/heads/{branchName}",
                sha = sha
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"https://api.github.com/repos/{repoOwner}/{repoName}/git/refs",
                createBranchRequest
            );

            return response.IsSuccessStatusCode;
        }
    }
}