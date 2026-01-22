using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.DataAccess.Concrete;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Hubs;
using static System.Net.Mime.MediaTypeNames;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ConnectionHub> _hubContext;
        private readonly IUserService _userService;
        private readonly MailService _emailService;
        private readonly SignInManager<CustomUser> _signInManager;
        private static readonly Dictionary<string, int> _verificationCodes = new();
        private readonly IFileService _fileService;
        private readonly IRecentActivityService recentActivityService;

        public ProfileController(UserManager<CustomUser> userManager, IConfiguration configuration, IHubContext<ConnectionHub> hubContext,
            IUserService userService, SignInManager<CustomUser> signInManager, MailService emailService, IFileService fileService, IRecentActivityService recentActivityService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _hubContext = hubContext;
            _userService = userService;
            _signInManager = signInManager;
            _emailService = emailService;
            _fileService = fileService;
            this.recentActivityService = recentActivityService;
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserProfile(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new
            {
                Username = user.UserName,
                Firstname = user.Firstname,
                Fullname = user.Firstname + " " + user.Lastname,
                Lastname = user.Lastname,
                Phone = user.PhoneNumber,
                Gender = user.Gender,
                Country = user.Country,
                Birthday = user.Birthday,
                Email = user.Email,
                Path = user.Image,
                Occupation = user.Occupation,

            });
        }

        [HttpGet("BasicInfoForProfil/{email}")]
        public async Task<IActionResult> GetBasicInfoForProfil(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new
            {
                Username = user.UserName,
                Firstname = user.Firstname,
                Fullname = user.Firstname + " " + user.Lastname,
                Lastname = user.Lastname,
                Phone = user.PhoneNumber,
                Gender = user.Gender,
                Country = user.Country,
                Birthday = user.Birthday,
                Email = user.Email,
                Path = user.Image,
                Occupation = user.Occupation,
                RegisterDate = user.RegisterDate,
                IsOnline = user.IsOnline,


            });
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Ok(new { Message = "User not authenticated.", Code = -1 });
            }
            var user = await _userService.GetUserById(userId);
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, value.OldPassword);
            if (isPasswordCorrect && value.NewPassword == value.ConfirmPassword)
            {
                await _userManager.ChangePasswordAsync(user, value.OldPassword, value.NewPassword);
                return Ok(new { Message = "Change password succesfuly" });
            }

            return Ok(new { Message = "Error", Code = -1 });

        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto value)
        {
            //maili gonderirsen eger dogrudursa true qaytarir
            var isCheckUser = await _userService.CheckUsernameOrEmail(value.NameOrEmail);
            if (!isCheckUser) return Ok(new { Result = false, Message = "This Mail Does Not Exist!" });

            var code = _emailService.sendVerifyMail(value.NameOrEmail);
            _verificationCodes[value.NameOrEmail] = code;

            // Mail göndermek hissesini yaz,code -u ora gonder

            return Ok(new { Result = true, Message = "Verification code sent" });

        }
        [HttpPost("verify-code")]//4 reqemli kod duzdurse

        public IActionResult VerifyCode(VerifyCodeDto model)
        {
            if (_verificationCodes.TryGetValue(model.Email, out var code))
            {
                return Ok(new { Result = true, Message = "Code verified" });
            }
            return Ok(new { Results = false, Message = "Invalid code" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Ok(new { message = "User not found" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                _verificationCodes.Remove(model.Email);
                return Ok(new { Result = true, Message = "Password reset successful" });
            }
            return Ok(new { Result = false, Message = result.Errors });
        }

        [HttpPost("email-confirmation")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ForgotPasswordDto value)
        {
            var isCheckUser = await _userService.CheckUsernameOrEmail(value.NameOrEmail);
            if (isCheckUser) return Ok(new { Result = false, Message = "This Mail Does Not Exist!" });

            var code = _emailService.sendVerifyMail(value.NameOrEmail);
            _verificationCodes[value.NameOrEmail] = code;

            // Mail göndermek hissesini yaz,code -u ora gonder

            return Ok(new { Result = true, Message = "Verification code sent" });
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.IsOnline = false;
            await _userService.Update(user);

            await _signInManager.SignOutAsync();
            await _hubContext.Clients.All.SendAsync("UpdateUserActivity");

            return Ok(new { message = "Logout successful" });
        }


        [Authorize]
        [HttpPut("EditedProfile")]
        public async Task<IActionResult> EditProfile([FromBody] UserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data provided." });
            }

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var temp = dto.Fullname?.Split(" ");
            user.Firstname = temp != null && temp.Length > 0 ? temp[0] : user.Firstname;
            user.Lastname = temp != null && temp.Length > 1 ? temp[1] : user.Lastname;

            user.Birthday = dto.Birthday;
            user.Email = dto.Email;
            user.Country = dto.Country;
            user.PhoneNumber = dto.Phone;
            user.Occupation = dto.Occupation;
            user.Gender = dto.Gender;

            await _userService.Update(user);
            await _hubContext.Clients.User(userId).SendAsync("ProfileUpdated");
            var item = new RecentActivity
            {
                UserId = userId,
                Text = "Profile updated succesfullly",
                Type = "Profile",
            };
            await recentActivityService.Add(item);
            await _hubContext.Clients.User(userId).SendAsync("RecentActivityUpdate1");

            return Ok(new { message = "Edit successful" });
        }

        [Authorize]
        [HttpPut("EditedProfileImage")]
        public async Task<IActionResult> EditProfileImage([FromForm] IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data provided." });
            }

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }
            if (file != null)
            {
                var filePath = await _fileService.SaveFile(file);
                user.Image = filePath;
            }

            await _userService.Update(user);
            await _hubContext.Clients.User(userId).SendAsync("ProfileUpdated");
            var item = new RecentActivity
            {
                UserId = userId,
                Text = "Profile image updated succesfullly",
                Type = "Profile",
            };
            await recentActivityService.Add(item);
            await _hubContext.Clients.User(userId).SendAsync("RecentActivityUpdate1");
            return Ok(new { message = "Edit successful" });
        }

        [Authorize]
        [HttpPut("AddingOccupationDuringQuiz")]

        public async Task<IActionResult> AddOccupationDuringQuiz([FromBody] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data provided." });
            }
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }
            user.Occupation = dto.Occupation;
            await _userService.Update(user);



            return Ok(new { message = "Add occupation successfully" });
        }

    }

}