using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Auth
{
    /// <summary>
    /// Qeydiyyat, sistemə giriş və hesabın silinməsi.
    /// </summary>
    public interface IAuthAppService
    {
        Task<ServiceResult<object>> SignUpAsync(SignUpDto dto);
        Task<ServiceResult<object>> SignInAsync(SignInDto dto);
        Task<ServiceResult<object>> DeleteAccountAsync(string userId);
    }
}
