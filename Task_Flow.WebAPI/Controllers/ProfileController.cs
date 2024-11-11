﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.DataAccess.Concrete;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Hubs;

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
        private readonly Dictionary<string, string> _verificationCodes = new();
        private readonly IFileService _fileService;

        public ProfileController(UserManager<CustomUser> userManager, IConfiguration configuration, IHubContext<ConnectionHub> hubContext, 
            IUserService userService, SignInManager<CustomUser> signInManager, MailService emailService,IFileService fileService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _hubContext = hubContext;
            _userService = userService;
            _signInManager = signInManager;
            _emailService = emailService;
            _fileService = fileService;
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
            if (isPasswordCorrect && value.NewPassword==value.ConfirmPassword)
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
            if (isCheckUser) return Ok("find email succesful");

            var code = new Random().Next(1000, 9999).ToString();
            _verificationCodes[value.NameOrEmail] = code;

            // Mail göndermek hissesini yaz,code -u ora gonder

            return Ok("Verification code sent");

        }
        [HttpPost("verify-code")]//4 reqemli kod duzdurse

        public IActionResult VerifyCode(VerifyCodeDto model)
        {
            if (_verificationCodes.TryGetValue(model.Email, out var code) && code == model.Code)
            {
                return Ok("Code verified");
            }
            return BadRequest("Invalid code");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return NotFound(new { message = "User not found" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                _verificationCodes.Remove(model.Email);
                return Ok(new { message = "Password reset successful" });
            }
            return BadRequest(result.Errors);
        }

        [Authorize]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "user not found" });
            }
            await _hubContext.Clients.All.SendAsync("UserDisconnected", userId);
            var user = await _userService.GetUserById(userId);
            user.IsOnline = false;
            await _userService.Update(user);
            await _signInManager.SignOutAsync();
            return Ok(new { message = "logout succesfuly" });
        }

        [Authorize]
        [HttpPost("EditedProfile")]
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
            return Ok(new { message = "Edit successful" });
        }

        [Authorize]
        [HttpPost("EditedProfileImage")]
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
            return Ok(new { message = "Edit successful" });
        }

    }

}
