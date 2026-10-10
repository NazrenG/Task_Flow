using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Auth
{
    public class AuthAppService : IAuthAppService
    {
        private const string DefaultOccupation = "Other (please specify)";

        private readonly UserManager<CustomUser> _userManager;
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IProfileRealtimeNotifier _notifier;

        public AuthAppService(
            UserManager<CustomUser> userManager,
            IUserService userService,
            IJwtTokenService jwtTokenService,
            IProfileRealtimeNotifier notifier)
        {
            _userManager = userManager;
            _userService = userService;
            _jwtTokenService = jwtTokenService;
            _notifier = notifier;
        }

        public async Task<ServiceResult<object>> SignUpAsync(SignUpDto dto)
        {
            var user = new CustomUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                Firstname = dto.Firstname,
                Lastname = dto.Lastname
            };

            var result = await _userManager.CreateAsync(user, dto.Password!);
            if (result.Succeeded)
            {
                return ServiceResult<object>.Success(new { Status = "Success", Message = "User created successfully!" });
            }

            return ServiceResult<object>.Success(new { Status = "Error", Message = "User creation failed!", Errors = result.Errors });
        }

        public async Task<ServiceResult<object>> SignInAsync(SignInDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username!);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password!))
            {
                return ServiceResult<object>.Unauthorized();
            }

            user.IsOnline = true;
            await _userService.Update(user);
            await _notifier.NotifyUserConnectedAsync(user.UserName!);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenService.CreateToken(user, roles);

            await EnsureOccupationAsync(user);

            return ServiceResult<object>.Success(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                PlanType = user.PlanType
            });
        }

        public async Task<ServiceResult<object>> DeleteAccountAsync(string userId)
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<object>.NotFound(new { message = "User not found" });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return ServiceResult<object>.Success(new { message = "Account deleted successfully" });
            }

            return ServiceResult<object>.BadRequest(new { message = "Failed to delete account", errors = result.Errors });
        }

        // Peşəsi qeyd olunmayan istifadəçiyə default dəyər verilir
        private async Task EnsureOccupationAsync(CustomUser user)
        {
            if (user.Occupation == null)
            {
                user.Occupation = DefaultOccupation;
                await _userService.Update(user);
            }
        }
    }
}
