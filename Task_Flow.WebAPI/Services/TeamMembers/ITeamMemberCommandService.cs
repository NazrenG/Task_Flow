using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.TeamMembers
{
    /// <summary>
    /// Komanda üzvünü birbaşa əlavə edən, dəyişən və layihədən çıxaran əməliyyatlar.
    /// </summary>
    public interface ITeamMemberCommandService
    {
        Task<ServiceResult<TeamMember>> AddMemberAsync(TeamMemberDto value);
        Task<ServiceResult<Empty>> ChangeMemberUserAsync(int id, string userId);
        Task<ServiceResult<Empty>> DeleteAsync(int id);
        Task<ServiceResult<Empty>> RemoveFromProjectAsync(string? currentUserId, RemoveMemberDto dto);
    }
}
