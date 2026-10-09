using Microsoft.AspNetCore.Identity;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Profiles
{
    public class PasswordAppService : IPasswordAppService
    {
        private const string MailNotExistMessage = "This Mail Does Not Exist!";
        private const string CodeSentMessage = "Verification code sent";

        private readonly IUserService _userService;
        private readonly UserManager<CustomUser> _userManager;
        private readonly MailService _mailService;
        private readonly IVerificationCodeStore _codeStore;

        public PasswordAppService(
            IUserService userService,
            UserManager<CustomUser> userManager,
            MailService mailService,
            IVerificationCodeStore codeStore)
        {
            _userService = userService;
            _userManager = userManager;
            _mailService = mailService;
            _codeStore = codeStore;
        }

        public async Task<ServiceResult<object>> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userService.GetUserById(userId);
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, dto.OldPassword);

            if (isPasswordCorrect && dto.NewPassword == dto.ConfirmPassword)
            {
                await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
                return ServiceResult<object>.Success(new { Message = "Change password succesfuly" });
            }

            return ServiceResult<object>.Success(new { Message = "Error", Code = -1 });
        }

        // Mail mövcuddursa, doğrulama kodu göndərilir
        public async Task<ServiceResult<object>> SendPasswordResetCodeAsync(ForgotPasswordDto dto)
        {
            var userExists = await _userService.CheckUsernameOrEmail(dto.NameOrEmail!);
            if (!userExists)
            {
                return ServiceResult<object>.Success(new { Result = false, Message = MailNotExistMessage });
            }

            SendVerificationCode(dto.NameOrEmail!);
            return ServiceResult<object>.Success(new { Result = true, Message = CodeSentMessage });
        }

        public ServiceResult<object> VerifyCode(VerifyCodeDto dto)
        {
            if (_codeStore.Contains(dto.Email))
            {
                return ServiceResult<object>.Success(new { Result = true, Message = "Code verified" });
            }

            return ServiceResult<object>.Success(new { Results = false, Message = "Invalid code" });
        }

        public async Task<ServiceResult<object>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return ServiceResult<object>.Success(new { message = "User not found" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (result.Succeeded)
            {
                _codeStore.Remove(dto.Email);
                return ServiceResult<object>.Success(new { Result = true, Message = "Password reset successful" });
            }

            return ServiceResult<object>.Success(new { Result = false, Message = result.Errors });
        }

        // Qeydiyyat zamanı: mail artıq istifadə olunursa kod göndərilmir
        public async Task<ServiceResult<object>> SendEmailConfirmationCodeAsync(ForgotPasswordDto dto)
        {
            var userExists = await _userService.CheckUsernameOrEmail(dto.NameOrEmail!);
            if (userExists)
            {
                return ServiceResult<object>.Success(new { Result = false, Message = MailNotExistMessage });
            }

            SendVerificationCode(dto.NameOrEmail!);
            return ServiceResult<object>.Success(new { Result = true, Message = CodeSentMessage });
        }

        private void SendVerificationCode(string email)
        {
            var code = _mailService.sendVerifyMail(email);
            _codeStore.Save(email, code);
        }
    }
}
