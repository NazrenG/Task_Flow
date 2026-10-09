using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Profiles
{
    /// <summary>
    /// Şifrə dəyişmə, şifrə bərpası və e-poçt təsdiqi axını.
    /// </summary>
    public interface IPasswordAppService
    {
        Task<ServiceResult<object>> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<ServiceResult<object>> SendPasswordResetCodeAsync(ForgotPasswordDto dto);
        ServiceResult<object> VerifyCode(VerifyCodeDto dto);
        Task<ServiceResult<object>> ResetPasswordAsync(ResetPasswordDto dto);
        Task<ServiceResult<object>> SendEmailConfirmationCodeAsync(ForgotPasswordDto dto);
    }
}
