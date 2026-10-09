using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Profiles;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileAppService _profileService;
        private readonly IPasswordAppService _passwordService;

        public ProfileController(IProfileAppService profileService, IPasswordAppService passwordService)
        {
            _profileService = profileService;
            _passwordService = passwordService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        private IActionResult InvalidData() => BadRequest(new { message = "Invalid data provided." });

        private IActionResult UserNotAuthenticated() => BadRequest(new { message = "User not authenticated." });

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> ViewProfile()
        {
            try
            {
                var userId = CurrentUserId;
                if (string.IsNullOrEmpty(userId)) return Unauthorized(new { message = "User not authenticated" });

                return this.ToActionResult(await _profileService.GetOwnProfileAsync(userId));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Profile Error: {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("disconnect-github")]
        public async Task<IActionResult> DisconnectGitHub()
        {
            try
            {
                return this.ToActionResult(await _profileService.DisconnectGitHubAsync(CurrentUserId));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Disconnect GitHub Error: {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserProfile(string email)
        {
            return this.ToActionResult(await _profileService.GetPublicProfileAsync(email));
        }

        [HttpGet("BasicInfoForProfil/{email}")]
        public async Task<IActionResult> GetBasicInfoForProfil(string email)
        {
            return this.ToActionResult(await _profileService.GetBasicInfoAsync(email));
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto value)
        {
            var userId = CurrentUserId;
            if (userId == null) return Ok(new { Message = "User not authenticated.", Code = -1 });

            return this.ToActionResult(await _passwordService.ChangePasswordAsync(userId, value));
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto value)
        {
            return this.ToActionResult(await _passwordService.SendPasswordResetCodeAsync(value));
        }

        // 4 reqemli kod duzdurse
        [HttpPost("verify-code")]
        public IActionResult VerifyCode(VerifyCodeDto model)
        {
            return this.ToActionResult(_passwordService.VerifyCode(model));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            return this.ToActionResult(await _passwordService.ResetPasswordAsync(model));
        }

        [HttpPost("email-confirmation")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ForgotPasswordDto value)
        {
            return this.ToActionResult(await _passwordService.SendEmailConfirmationCodeAsync(value));
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = CurrentUserId;
            if (userId == null) return BadRequest(new { message = "User not found" });

            return this.ToActionResult(await _profileService.LogoutAsync(userId));
        }

        [Authorize]
        [HttpPut("EditedProfile")]
        public async Task<IActionResult> EditProfile([FromBody] UserDto dto)
        {
            if (!ModelState.IsValid) return InvalidData();

            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _profileService.EditProfileAsync(userId, dto));
        }

        [Authorize]
        [HttpPut("EditedProfileImage")]
        public async Task<IActionResult> EditProfileImage([FromForm] IFormFile file)
        {
            if (!ModelState.IsValid) return InvalidData();

            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _profileService.EditProfileImageAsync(userId, file));
        }

        [Authorize]
        [HttpPut("AddingOccupationDuringQuiz")]
        public async Task<IActionResult> AddOccupationDuringQuiz([FromBody] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid) return InvalidData();

            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _profileService.AddOccupationAsync(userId, dto));
        }
    }
}
