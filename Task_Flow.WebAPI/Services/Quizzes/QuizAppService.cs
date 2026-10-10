using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Quizzes
{
    public class QuizAppService : IQuizAppService
    {
        private readonly IQuizService _quizService;

        public QuizAppService(IQuizService quizService)
        {
            _quizService = quizService;
        }

        public async Task<ServiceResult<Empty>> CreateQuizAsync(QuizDto dto)
        {
            await _quizService.Add(new Quiz
            {
                Profession = dto.Profession,
                UsagePurpose = dto.UsagePurpose
            });

            return ServiceResult<Empty>.Success(Empty.Value);
        }
    }
}
