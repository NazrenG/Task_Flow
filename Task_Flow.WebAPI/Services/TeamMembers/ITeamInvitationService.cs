using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.TeamMembers
{
    /// <summary>
    /// İstifadəçiləri layihəyə dəvət etmək və dəvətləri ləğv etmək.
    /// </summary>
    public interface ITeamInvitationService
    {
        Task<ServiceResult<object>> InviteMembersAsync(string? senderId, TeamMemberCollectionDto dto);
        Task<ServiceResult<object>> ReplaceTeamAndInviteAsync(string? senderId, TeamMemberCollectionDto dto);
        Task<ServiceResult<object>> CancelInvitationAsync(RemoveTeamMemberDto dto);
    }
}
