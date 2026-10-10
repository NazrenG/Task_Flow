using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Auth;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthAppService _authService;
        private readonly IUserLookupService _userLookupService;

        public AuthController(IAuthAppService authService, IUserLookupService userLookupService)
        {
            _authService = authService;
            _userLookupService = userLookupService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [Authorize]
        [HttpPost("searchedUser")]
        public async Task<IActionResult> SearchUser([FromBody] SearchedUserDto dto)
        {
            return this.ToActionResult(await _userLookupService.SearchUsersAsync(CurrentUserId, dto.Key));
        }

        [HttpPost("GetUserWithUsername")]
        public async Task<IActionResult> GetUserWithUsername(string username)
        {
            return this.ToActionResult(await _userLookupService.GetUsersByNameAsync(username));
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpDto dto)
        {
            return this.ToActionResult(await _authService.SignUpAsync(dto));
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto dto)
        {
            return this.ToActionResult(await _authService.SignInAsync(dto));
        }

        [Authorize]
        [HttpGet("currentUser")]
        public async Task<IActionResult> GetCurrentUserData()
        {
            var userId = CurrentUserId;
            if (userId == null) return NotFound(new { Message = "user not found" });

            return this.ToActionResult(await _userLookupService.GetCurrentUserAsync(userId));
        }

        [HttpGet("UsersCount")]
        public async Task<IActionResult> GetUserCount()
        {
            return this.ToActionResult(await _userLookupService.GetUserCountAsync());
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { message = "User is not authenticated" });

            return this.ToActionResult(await _authService.DeleteAccountAsync(userId));
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserWithEmail(string email)
        {
            return this.ToActionResult(await _userLookupService.GetUserByEmailAsync(email));
        }

        [HttpGet("test")]
        public IActionResult test()
        {
            return Ok("helloo!");
        }
    }
}
