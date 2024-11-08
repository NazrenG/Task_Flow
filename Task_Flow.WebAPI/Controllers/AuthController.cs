﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.DataAccess.Concrete;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IQuizService _quizService;
        private readonly IUserService _userService;

        public AuthController(UserManager<CustomUser> userManager, IConfiguration configuration, IQuizService quizService, IUserService userService)
        {
            _userManager = userManager;
            _quizService = quizService;
            _userService = userService;
            _configuration = configuration;
        }


        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpDto dto)
        {
            var user = new CustomUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                Firstname = dto.Firstname,
                Lastname = dto.Lastname,
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                await _quizService.Add(new Quiz());
                return Ok(new { Status = "Success", Message = "User created successfully!" });
            }

            return BadRequest(new { Status = "Error", Message = "User creation failed!", Errors = result.Errors });
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                     new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };

                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var token = GetToken(authClaims);

                return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token), Expiration = token.ValidTo });
            }

            return Unauthorized();
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
        [Authorize]
        [HttpGet("currentUser")]
        public async Task<IActionResult> GetCurrentUserData()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return NotFound(new { Message = "user not found" });
            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                return NotFound(new { Status = "Error", Message = "User not found!" });
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

        [HttpGet("UsersCount")]
        public async Task<IActionResult> GetUserCount()
        {
            var count = await _userService.GetAllUserCount();

            return Ok(count);
        }
    }
}
