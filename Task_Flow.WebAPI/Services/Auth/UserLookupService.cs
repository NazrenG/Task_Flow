using Microsoft.AspNetCore.Identity;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Auth
{
    public class UserLookupService : IUserLookupService
    {
        private readonly IUserService _userService;
        private readonly UserManager<CustomUser> _userManager;

        public UserLookupService(IUserService userService, UserManager<CustomUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        // Axtarış nəticəsindən istifadəçinin özü çıxarılır
        public async Task<ServiceResult<object>> SearchUsersAsync(string? currentUserId, string key)
        {
            var users = await _userService.GetUserByName(key);
            var others = users.Where(u => u.Id != currentUserId).ToList();

            return ServiceResult<object>.Success(new { Users = others });
        }

        public async Task<ServiceResult<List<CustomUser>>> GetUsersByNameAsync(string username)
        {
            return ServiceResult<List<CustomUser>>.Success(await _userService.GetUserByName(username));
        }

        public async Task<ServiceResult<object>> GetCurrentUserAsync(string userId)
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<object>.NotFound(new { Status = "Error", Message = "User not found!" });
            }

            return ServiceResult<object>.Success(user.ToCurrentUserData());
        }

        public async Task<ServiceResult<int>> GetUserCountAsync()
        {
            return ServiceResult<int>.Success(await _userService.GetAllUserCount());
        }

        public async Task<ServiceResult<CustomUser>> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return ServiceResult<CustomUser>.BadRequest("Email is required.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ServiceResult<CustomUser>.NotFound($"No user found with the email: {email}");
            }

            return ServiceResult<CustomUser>.Success(user);
        }
    }
}
