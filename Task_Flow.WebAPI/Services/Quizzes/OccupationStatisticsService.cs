using Task_Flow.DataAccess.Abstract;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Quizzes
{
    public class OccupationStatisticsService : IOccupationStatisticsService
    {
        private const int PercentageDecimals = 2;

        private readonly IQuizService _quizService;
        private readonly IProjectService _projectService;
        private readonly ITeamMemberService _teamMemberService;

        public OccupationStatisticsService(
            IQuizService quizService,
            IProjectService projectService,
            ITeamMemberService teamMemberService)
        {
            _quizService = quizService;
            _projectService = projectService;
            _teamMemberService = teamMemberService;
        }

        // Hər peşə üzrə istifadəçi sayı quiz-lərin ümumi sayına nisbətdə
        public async Task<ServiceResult<List<OccupationStatisticDto>>> GetOverallStatisticsAsync()
        {
            var quizzes = await _quizService.Quizzes();
            var totalCount = quizzes.Count;

            var statistics = new List<OccupationStatisticDto>();
            foreach (var occupation in QuizOptions.Occupations)
            {
                var count = await _quizService.SpecialOccupationCount(occupation);
                statistics.Add(CreateStatistic(occupation, count, totalCount));
            }

            return ServiceResult<List<OccupationStatisticDto>>.Success(statistics);
        }

        // İstifadəçinin layihələrindəki komanda üzvlərinin peşə bölgüsü
        public async Task<ServiceResult<List<OccupationStatisticDto>>> GetProjectMembersStatisticsAsync(string userId)
        {
            var projects = await _projectService.GetProjects(userId);
            if (projects == null || !projects.Any())
            {
                return ServiceResult<List<OccupationStatisticDto>>.Success(CreateEmptyStatistics());
            }

            var projectIds = projects.Select(p => p.Id).ToList();
            var members = await _teamMemberService.GetUsersByProjectIdsAsync(projectIds);
            if (members == null || !members.Any())
            {
                return ServiceResult<List<OccupationStatisticDto>>.Success(CreateEmptyStatistics());
            }

            var statistics = QuizOptions.Occupations
                .Select(occupation => CreateStatistic(
                    occupation,
                    members.Count(m => m.Occupation == occupation),
                    members.Count))
                .ToList();

            return ServiceResult<List<OccupationStatisticDto>>.Success(statistics);
        }

        private static List<OccupationStatisticDto> CreateEmptyStatistics()
        {
            return QuizOptions.Occupations
                .Select(occupation => new OccupationStatisticDto { OccupationName = occupation, Percentage = 0 })
                .ToList();
        }

        private static OccupationStatisticDto CreateStatistic(string occupation, int count, int totalCount)
        {
            return new OccupationStatisticDto
            {
                OccupationName = occupation,
                Percentage = CalculatePercentage(count, totalCount)
            };
        }

        private static decimal CalculatePercentage(int count, int totalCount)
        {
            return totalCount > 0 ? Math.Round(count * 100m / totalCount, PercentageDecimals) : 0;
        }
    }
}
