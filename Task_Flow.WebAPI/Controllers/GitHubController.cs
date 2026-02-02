using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;
        private readonly IUserService userService;
        private readonly IConfiguration _configuration;

        public GitHubController(IGitHubService gitHubService, IUserService userService, IConfiguration configuration)
        {
            _gitHubService = gitHubService;
            this.userService = userService;
            _configuration = configuration;
        }
        [Authorize]
        [HttpGet("authorize")]
        public async Task<IActionResult> Authorize()
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var authUrl = await _gitHubService.GetAuthorizationUrl(userId);

                return Ok(new { authUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GitHub Authorize Error: {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return BadRequest("Authorization code is missing");

                var accessToken = await _gitHubService.ExchangeCodeForToken(code, state);

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "YourApp");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var userResponse = await httpClient.GetAsync("https://api.github.com/user");
                userResponse.EnsureSuccessStatusCode();

                var githubUser = await userResponse.Content.ReadFromJsonAsync<JsonElement>();
                var githubUsername = githubUser.GetProperty("login").GetString();

                var user = await userService.GetUserById(state);
                if (user != null)
                {
                    user.GitHubAccessToken = accessToken;
                    user.GitHubUsername = githubUsername;
                    await userService.Update(user);
                }

                // Frontend-ə redirect
                var redirectUrl = $"{_configuration["FrontendUrl"]}/profile?github=success";
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GitHub Callback Error: {ex.Message}");
                var redirectUrl = $"{_configuration["FrontendUrl"]}/profile?github=error";
                return Redirect(redirectUrl);
            }
        }
    }
    }
