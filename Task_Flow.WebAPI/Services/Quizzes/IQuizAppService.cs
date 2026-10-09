using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Quizzes
{
    /// <summary>
    /// İstifadəçinin doldurduğu quiz cavablarını saxlayır.
    /// </summary>
    public interface IQuizAppService
    {
        Task<ServiceResult<Empty>> CreateQuizAsync(QuizDto dto);
    }
}
