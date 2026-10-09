using Microsoft.AspNetCore.Identity;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.NotificationCenter;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Profiles
{
    public class ProfileAppService : IProfileAppService
    {
        private const string ProfileActivityType = "Profile";
        private const string EditSuccessMessage = "Edit successful";

        private readonly IUserService _userService;
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IFileService _fileService;
        private readonly IRecentActivityAppService _recentActivityService;
        private readonly IProfileRealtimeNotifier _notifier;

        public ProfileAppService(
            IUserService userService,
            UserManager<CustomUser> userManager,
            SignInManager<CustomUser> signInManager,
            IFileService fileService,
            IRecentActivityAppService recentActivityService,
            IProfileRealtimeNotifier notifier)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
            _fileService = fileService;
            _recentActivityService = recentActivityService;
            _notifier = notifier;
        }

        public async Task<ServiceResult<object>> GetOwnProfileAsync(string userId)
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<object>.NotFound(new { message = "User not found" });
            }

            return ServiceResult<object>.Success(user.ToOwnProfile(userId));
        }

        public async Task<ServiceResult<object>> GetPublicProfileAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ServiceResult<object>.NotFound("User not found");
            }

            return ServiceResult<object>.Success(user.ToPublicProfile());
        }

        public async Task<ServiceResult<object>> GetBasicInfoAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ServiceResult<object>.NotFound("User not found");
            }

            return ServiceResult<object>.Success(user.ToBasicProfileInfo());
        }

        public async Task<ServiceResult<object>> DisconnectGitHubAsync(string? userId)
        {
            var user = await _userService.GetUserById(userId!);
            if (user != null)
            {
                user.GitHubAccessToken = null;
                user.GitHubUsername = null;
                await _userService.Update(user);
            }

            return ServiceResult<object>.Success(new { message = "GitHub disconnected successfully" });
        }

        public async Task<ServiceResult<object>> EditProfileAsync(string userId, UserDto dto)
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return UserNotFound();
            }

            user.ApplyProfileEdit(dto);
            await SaveProfileAsync(user, userId, "Profile updated succesfullly");

            return ServiceResult<object>.Success(new { message = EditSuccessMessage });
        }

        public async Task<ServiceResult<object>> EditProfileImageAsync(string userId, IFormFile? file)
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return UserNotFound();
            }

            if (file != null)
            {
                user.Image = await _fileService.SaveFile(file);
            }

            await SaveProfileAsync(user, userId, "Profile image updated succesfullly");

            return ServiceResult<object>.Success(new { message = EditSuccessMessage });
        }

        public async Task<ServiceResult<object>> AddOccupationAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return UserNotFound();
            }

            user.Occupation = dto.Occupation;
            await _userService.Update(user);

            return ServiceResult<object>.Success(new { message = "Add occupation successfully" });
        }

        public async Task<ServiceResult<object>> LogoutAsync(string userId)
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return ServiceResult<object>.NotFound(new { message = "User not found" });
            }

            user.IsOnline = false;
            await _userService.Update(user);

            await _signInManager.SignOutAsync();
            await _notifier.NotifyUserActivityChangedForAllAsync();

            return ServiceResult<object>.Success(new { message = "Logout successful" });
        }

        // Profil dəyişikliyini saxlayır, ekranı yeniləyir və activity log yazır
        private async Task SaveProfileAsync(CustomUser user, string userId, string activityText)
        {
            await _userService.Update(user);
            await _notifier.NotifyProfileUpdatedAsync(userId);
            await _recentActivityService.LogActivityAsync(userId, activityText, ProfileActivityType);
        }

        private static ServiceResult<object> UserNotFound()
        {
            return ServiceResult<object>.NotFound(new { message = "User not found." });
        }
    }
}
