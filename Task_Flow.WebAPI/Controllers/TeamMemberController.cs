using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.TeamMembers;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamMemberController : ControllerBase
    {
        private readonly ITeamMemberQueryService _teamMemberQueryService;
        private readonly ITeamMemberCommandService _teamMemberCommandService;
        private readonly ITeamInvitationService _teamInvitationService;

        public TeamMemberController(
            ITeamMemberQueryService teamMemberQueryService,
            ITeamMemberCommandService teamMemberCommandService,
            ITeamInvitationService teamInvitationService)
        {
            _teamMemberQueryService = teamMemberQueryService;
            _teamMemberCommandService = teamMemberCommandService;
            _teamInvitationService = teamInvitationService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet("AllMember")]
        public async Task<IEnumerable<TeamMemberDto>> Get()
        {
            return await _teamMemberQueryService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMember(int id)
        {
            return this.ToActionResult(await _teamMemberQueryService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TeamMemberDto value)
        {
            if (value == null) return BadRequest();

            return this.ToActionResult(await _teamMemberCommandService.AddMemberAsync(value));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] string value)
        {
            return this.ToActionResult(await _teamMemberCommandService.ChangeMemberUserAsync(id, value));
        }

        // DELETE api/<TeamMemberController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return this.ToActionResult(await _teamMemberCommandService.DeleteAsync(id));
        }

        [Authorize]
        [HttpPost("UpdateTeamMemberCollections")]
        public async Task<IActionResult> UpdateTeamMembersAsTeam([FromBody] TeamMemberCollectionDto dto)
        {
            try
            {
                return this.ToActionResult(await _teamInvitationService.ReplaceTeamAndInviteAsync(CurrentUserId, dto));
            }
            catch (Exception ex)
            {
                return TeamMembersError(ex);
            }
        }

        [HttpGet("GetUsersByProject/{projectId}")]
        public async Task<IActionResult> GetUsersByProject(int projectId)
        {
            return this.ToActionResult(await _teamMemberQueryService.GetUsersByProjectAsync(projectId));
        }

        [Authorize]
        [HttpPost("TeamMemberCollections")]
        public async Task<IActionResult> AddTeamMembersAsTeam([FromBody] TeamMemberCollectionDto dto)
        {
            try
            {
                return this.ToActionResult(await _teamInvitationService.InviteMembersAsync(CurrentUserId, dto));
            }
            catch (Exception ex)
            {
                return TeamMembersError(ex);
            }
        }

        [Authorize]
        [HttpDelete("TeammemberRemover")]
        public async Task<IActionResult> RemoveMember([FromBody] RemoveTeamMemberDto dto)
        {
            return this.ToActionResult(await _teamInvitationService.CancelInvitationAsync(dto));
        }

        [Authorize]
        [HttpDelete("MemberRemove")]
        public async Task<IActionResult> RemoveTM([FromBody] RemoveMemberDto dto)
        {
            return this.ToActionResult(await _teamMemberCommandService.RemoveFromProjectAsync(CurrentUserId, dto));
        }

        [Authorize]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetMembersByProjectId([FromRoute] int id)
        {
            return this.ToActionResult(await _teamMemberQueryService.GetMembersWithInvitationsAsync(id));
        }

        private IActionResult TeamMembersError(Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while adding team members.", Details = ex.Message });
        }
    }
}
