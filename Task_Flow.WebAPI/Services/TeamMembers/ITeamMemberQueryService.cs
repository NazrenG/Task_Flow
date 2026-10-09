using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.TeamMembers
{
    /// <summary>
    /// Layihə komanda üzvlərini oxuyan əməliyyatlar.
    /// </summary>
    public interface ITeamMemberQueryService
    {
        Task<List<TeamMemberDto>> GetAllAsync();
        Task<ServiceResult<TeamMemberDto>> GetByIdAsync(int id);
        Task<ServiceResult<List<TeamUserDto>>> GetUsersByProjectAsync(int projectId);
        Task<ServiceResult<object>> GetMembersWithInvitationsAsync(int projectId);
    }
}
