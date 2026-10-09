using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Services.GitHub;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private readonly IGitHubAccountService _gitHubAccountService;

        public GitHubController(IGitHubAccountService gitHubAccountService)
        {
            _gitHubAccountService = gitHubAccountService;
        }

        [Authorize]
        [HttpGet("authorize")]
        public async Task<IActionResult> Authorize()
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized(new { message = "User not authenticated" });

                return this.ToActionResult(await _gitHubAccountService.GetAuthorizationUrlAsync(userId));
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
                if (string.IsNullOrEmpty(code)) return BadRequest("Authorization code is missing");

                await _gitHubAccountService.ConnectAccountAsync(code, state);

                // Frontend-ə redirect
                return Redirect(_gitHubAccountService.GetProfileRedirectUrl(success: true));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GitHub Callback Error: {ex.Message}");
                return Redirect(_gitHubAccountService.GetProfileRedirectUrl(success: false));
            }
        }
    }
}
