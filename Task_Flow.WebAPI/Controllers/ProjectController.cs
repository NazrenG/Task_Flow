using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Projects;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private const string InvalidTokenMessage = "Invalid token or user not found.";

        private readonly IProjectQueryService _projectQueryService;
        private readonly IProjectCommandService _projectCommandService;

        public ProjectController(IProjectQueryService projectQueryService, IProjectCommandService projectCommandService)
        {
            _projectQueryService = projectQueryService;
            _projectCommandService = projectCommandService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet("ProjectTitle/{projectId}")]
        public async Task<IActionResult> GetProjectTitle(int projectId)
        {
            return this.ToActionResult(await _projectQueryService.GetProjectTitleAsync(projectId));
        }

        [Authorize]
        [HttpGet("ExtendedProjectList")]
        public async Task<IActionResult> GetExtendedProjectList()
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId)) return Unauthorized(InvalidTokenMessage);

            return this.ToActionResult(await _projectQueryService.GetExtendedProjectListAsync(userId));
        }

        [Authorize]
        [HttpGet("UserProjectCount")]
        public async Task<IActionResult> GetUserProjectCount()
        {
            var userId = CurrentUserId;
            if (userId == null) return Unauthorized(InvalidTokenMessage);

            return this.ToActionResult(await _projectQueryService.GetUserProjectCountAsync(userId));
        }

        [Authorize]
        [HttpPost("ProjectWithTitle")]
        public async Task<IActionResult> GetProjectWithTitle([FromBody] string title)
        {
            try
            {
                var userId = CurrentUserId;
                if (string.IsNullOrEmpty(userId)) return Unauthorized(new { message = "User not authorized." });

                return this.ToActionResult(await _projectQueryService.GetProjectByTitleAsync(userId, title));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            return this.ToActionResult(await _projectQueryService.GetProjectAsync(id));
        }

        // Pryektin icindeki tasklar CANBAN
        [HttpGet("ProjectTaskCanban/{projectId}")]
        public async Task<IActionResult> Get(int projectId)
        {
            return this.ToActionResult(await _projectQueryService.GetCanbanTasksAsync(projectId));
        }

        // canbanda tasklarin statusunu deyisdirmek
        [Authorize]
        [HttpPut("UpdateTaskColumn")]
        public async Task<IActionResult> UpdateTaskColumn([FromBody] UpdateTaskColumnDto dto)
        {
            return this.ToActionResult(await _projectCommandService.UpdateTaskColumnAsync(CurrentUserId, dto));
        }

        [Authorize]
        [HttpGet("AllProjectsUserOwn")]
        public async Task<IActionResult> GetUserOwnProjects()
        {
            return this.ToActionResult(await _projectQueryService.GetOwnProjectsAsync(CurrentUserId));
        }

        [Authorize]
        [HttpGet("UserAddedProjects")]
        public async Task<IActionResult> GetUserAddedProjects()
        {
            return this.ToActionResult(await _projectQueryService.GetAddedProjectsAsync(CurrentUserId));
        }

        [HttpGet("ProjectTaskCount/{id}")]
        public async Task<IActionResult> GetProjectTaskCount(int id)
        {
            return this.ToActionResult(await _projectQueryService.GetProjectTaskCountAsync(id));
        }

        // POST api/<ProjectController>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateProjectDto value)
        {
            return this.ToActionResult(await _projectCommandService.CreateProjectAsync(CurrentUserId, value));
        }

        [Authorize]
        [HttpPut("Put/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] PutProjectDto dto)
        {
            return this.ToActionResult(await _projectCommandService.UpdateProjectAsync(id, CurrentUserId, dto));
        }

        // PUT api/<ProjectController>/5
        [Authorize]
        [HttpPut("ChangeTitle/{id}")]
        public async Task<IActionResult> PutTitle(int id, [FromBody] string value)
        {
            return this.ToActionResult(await _projectCommandService.ChangeTitleAsync(id, CurrentUserId, value));
        }

        [Authorize]
        [HttpPut("ChangeDescription/{id}")]
        public async Task<IActionResult> PutDescription(int id, [FromBody] string value)
        {
            return this.ToActionResult(await _projectCommandService.ChangeDescriptionAsync(id, CurrentUserId, value));
        }

        [Authorize]
        [HttpPut("ChangeCompleted/{id}")]
        public async Task<IActionResult> PutCompleted(int id, [FromBody] bool value)
        {
            return this.ToActionResult(await _projectCommandService.ChangeCompletedAsync(id, CurrentUserId, value));
        }

        // DELETE api/<ProjectController>/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return this.ToActionResult(await _projectCommandService.DeleteProjectAsync(id, CurrentUserId));
        }

        [Authorize]
        [HttpGet("OnGoingProject")]
        public Task<IActionResult> GetOnGoingProject() => GetProjectsByStatus(ProjectStatusFilter.OnGoing);

        [Authorize]
        [HttpGet("PendingProject")]
        public Task<IActionResult> GetPendingProject() => GetProjectsByStatus(ProjectStatusFilter.Pending);

        [Authorize]
        [HttpGet("CompletedProject")]
        public Task<IActionResult> GetCompletedProject() => GetProjectsByStatus(ProjectStatusFilter.Completed);

        [Authorize]
        [HttpGet("OnGoingProjectCount")]
        public Task<IActionResult> GetOnGoingProjectCount() => GetProjectCountByStatus(ProjectStatusFilter.OnGoing);

        [Authorize]
        [HttpGet("PendingProjectCount")]
        public Task<IActionResult> GetPendingProjectCount() => GetProjectCountByStatus(ProjectStatusFilter.Pending);

        [Authorize]
        [HttpGet("CompletedTaskCount")]
        public Task<IActionResult> GetCompletedTaskCount() => GetProjectCountByStatus(ProjectStatusFilter.Completed);

        [Authorize]
        [HttpGet("ProjectNames")]
        public async Task<IActionResult> GetProjectNames()
        {
            var userId = CurrentUserId;
            if (userId == null) return Unauthorized(InvalidTokenMessage);

            return this.ToActionResult(await _projectQueryService.GetProjectNamesAsync(userId));
        }

        [Authorize]
        [HttpPost("TasksDependingMonths")]
        public async Task<IActionResult> GetTasksForChart([FromBody] string projectName)
        {
            var userId = CurrentUserId;
            if (userId == null) return Unauthorized(InvalidTokenMessage);

            return this.ToActionResult(await _projectQueryService.GetTaskChartAsync(userId, projectName));
        }

        // userin istirak etdiyi layiheler => dashboardda
        [Authorize]
        [HttpGet("ProjectInvolved")]
        public async Task<IActionResult> ProjectInvolved()
        {
            var userId = CurrentUserId;
            if (userId == null) return Unauthorized(InvalidTokenMessage);

            return this.ToActionResult(await _projectQueryService.GetInvolvedProjectsAsync(userId));
        }

        private async Task<IActionResult> GetProjectsByStatus(ProjectStatusFilter status)
        {
            var userId = CurrentUserId;
            if (userId == null) return Unauthorized(InvalidTokenMessage);

            return this.ToActionResult(await _projectQueryService.GetProjectsByStatusAsync(userId, status));
        }

        private async Task<IActionResult> GetProjectCountByStatus(ProjectStatusFilter status)
        {
            var userId = CurrentUserId;
            if (userId == null) return Unauthorized(InvalidTokenMessage);

            return this.ToActionResult(await _projectQueryService.GetProjectCountByStatusAsync(userId, status));
        }
    }
}
