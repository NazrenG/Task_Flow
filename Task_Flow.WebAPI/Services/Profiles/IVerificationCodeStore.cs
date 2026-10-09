namespace Task_Flow.WebAPI.Services.Profiles
{
    /// <summary>
    /// Şifrə bərpası və e-poçt təsdiqi üçün göndərilən doğrulama kodlarını saxlayır.
    /// </summary>
    public interface IVerificationCodeStore
    {
        void Save(string email, int code);
        bool Contains(string email);
        void Remove(string email);
    }
}
