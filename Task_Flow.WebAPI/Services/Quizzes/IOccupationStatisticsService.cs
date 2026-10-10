using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Quizzes
{
    /// <summary>
    /// Peşələr üzrə istifadəçi faizlərini hesablayır.
    /// </summary>
    public interface IOccupationStatisticsService
    {
        Task<ServiceResult<List<OccupationStatisticDto>>> GetOverallStatisticsAsync();
        Task<ServiceResult<List<OccupationStatisticDto>>> GetProjectMembersStatisticsAsync(string userId);
    }
}
