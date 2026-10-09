using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.GitHub
{
    /// <summary>
    /// İstifadəçinin GitHub hesabını OAuth ilə sistemə bağlayır.
    /// </summary>
    public interface IGitHubAccountService
    {
        Task<ServiceResult<object>> GetAuthorizationUrlAsync(string userId);

        // OAuth callback: kodu token-ə dəyişir və token-i istifadəçiyə yazır
        Task ConnectAccountAsync(string code, string userId);

        string GetProfileRedirectUrl(bool success);
    }
}
