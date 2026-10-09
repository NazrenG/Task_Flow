using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Profiles
{
    /// <summary>
    /// İstifadəçi profilinə baxış və profil redaktəsi.
    /// </summary>
    public interface IProfileAppService
    {
        Task<ServiceResult<object>> GetOwnProfileAsync(string userId);
        Task<ServiceResult<object>> GetPublicProfileAsync(string email);
        Task<ServiceResult<object>> GetBasicInfoAsync(string email);

        Task<ServiceResult<object>> DisconnectGitHubAsync(string? userId);
        Task<ServiceResult<object>> EditProfileAsync(string userId, UserDto dto);
        Task<ServiceResult<object>> EditProfileImageAsync(string userId, IFormFile? file);
        Task<ServiceResult<object>> AddOccupationAsync(string userId, UpdateProfileDto dto);
        Task<ServiceResult<object>> LogoutAsync(string userId);
    }
}
