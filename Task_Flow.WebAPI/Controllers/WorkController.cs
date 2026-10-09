using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Works;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        private readonly IWorkQueryService _workQueryService;
        private readonly IWorkCommandService _workCommandService;

        public WorkController(IWorkQueryService workQueryService, IWorkCommandService workCommandService)
        {
            _workQueryService = workQueryService;
            _workCommandService = workCommandService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        private IActionResult UserNotAuthenticated() => BadRequest(new { message = "User not authenticated." });

        // GET: api/<WorkController>
        [Authorize]
        [HttpGet("UserTasks")]
        public async Task<IActionResult> GetUserTasks()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workQueryService.GetUserTasksAsync(userId));
        }

        [HttpGet("UserProfileTask/{email}")]
        public async Task<IActionResult> GetUserProfileTask(string email)
        {
            return this.ToActionResult(await _workQueryService.GetUserProfileTasksAsync(email));
        }

        // GET api/<WorkController>/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workQueryService.GetTaskAsync(id, userId));
        }

        [Authorize]
        [HttpGet("FullWorkDetail/{id}")]
        public async Task<IActionResult> GetFullWorkData(int id)
        {
            if (CurrentUserId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workQueryService.GetFullWorkDetailAsync(id));
        }

        // PUT api/<WorkController>/5
        [Authorize]
        [HttpPut("EditedProjectTask/{id}")]
        public async Task<IActionResult> PutProjectTaskByUser(int id, [FromBody] WorkDto value)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workCommandService.UpdateTaskStatusAsync(id, userId, value));
        }

        // canban icinde tasklari edit etmek
        [Authorize]
        [HttpPut("EditedProjectForPmTask/{id}")]
        public async Task<IActionResult> PutProjectForPmTask(int id, [FromBody] WorkDto value)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workCommandService.UpdateTaskByManagerAsync(id, userId, value));
        }

        [Authorize]
        [HttpGet("UserTasksCount")]
        public async Task<IActionResult> GetUserTaskCount()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workQueryService.GetUserTaskCountAsync(userId));
        }

        [HttpGet("UserTasksCountForEmail/{email}")]
        public async Task<IActionResult> GetUserTaskCountForEmail(string email)
        {
            return this.ToActionResult(await _workQueryService.GetUserTaskCountByEmailAsync(email));
        }

        // POST api/<WorkController>
        [Authorize]
        [HttpPost("NewTask")]
        public async Task<IActionResult> Post([FromBody] WorkDto value)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workCommandService.CreateTaskAsync(userId, value));
        }

        // DELETE api/<WorkController>/5
        [Authorize]
        [HttpDelete("DeleteProjectTask/{taskId}")]
        public async Task<IActionResult> Delete(int taskId, [FromQuery] int projectId)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workCommandService.DeleteTaskAsync(taskId, projectId, userId));
        }

        [Authorize]
        [HttpGet("DailyTask")]
        public async Task<IActionResult> GetDailyTask()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workQueryService.GetDailyTasksAsync(userId));
        }

        [Authorize]
        [HttpGet("ToDoTask")]
        public Task<IActionResult> GetToDoTask() => GetTasksByStatus(WorkStatusFilter.ToDo);

        [Authorize]
        [HttpGet("ToDoTaskCount")]
        public Task<IActionResult> GetToDoTaskCount() => GetTaskCountByStatus(WorkStatusFilter.ToDo);

        [HttpGet("ToDoTaskCountForMail/{email}")]
        public Task<IActionResult> GetToDoTaskCountForMail(string email) =>
            GetTaskCountByStatusForEmail(email, WorkStatusFilter.ToDo);

        [Authorize]
        [HttpGet("InProgressTask")]
        public Task<IActionResult> GetInProgressTask() => GetTasksByStatus(WorkStatusFilter.InProgress);

        [Authorize]
        [HttpGet("InProgressTaskCount")]
        public Task<IActionResult> GetInProgressTaskCount() => GetTaskCountByStatus(WorkStatusFilter.InProgress);

        [HttpGet("InProgressTaskCountForEmail/{email}")]
        public Task<IActionResult> GetInProgressTaskCountForEmail(string email) =>
            GetTaskCountByStatusForEmail(email, WorkStatusFilter.InProgress);

        [Authorize]
        [HttpGet("DoneTask")]
        public Task<IActionResult> GetDoneTask() => GetTasksByStatus(WorkStatusFilter.Done);

        [Authorize]
        [HttpGet("DoneTaskCount")]
        public Task<IActionResult> GetDoneTaskCount() => GetTaskCountByStatus(WorkStatusFilter.Done);

        [HttpGet("DoneTaskCountForEmail/{email}")]
        public Task<IActionResult> GetDoneTaskCountForEmail(string email) =>
            GetTaskCountByStatusForEmail(email, WorkStatusFilter.Done);

        [Authorize]
        [HttpGet("UserWorks")]
        public async Task<IActionResult> GetUserWorks()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workQueryService.GetProjectOwnerWorksAsync(userId));
        }

        [Authorize]
        [HttpGet("ProjectWorks/{projectId}")]
        public async Task<IActionResult> GetProjectWorks(int projectId)
        {
            if (CurrentUserId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workQueryService.GetProjectWorksAsync(projectId));
        }

        [Authorize]
        [HttpGet("Backlogs/{projectId}")]
        public async Task<IActionResult> GetBacklogs(int projectId)
        {
            if (CurrentUserId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workQueryService.GetBacklogsAsync(projectId));
        }

        private async Task<IActionResult> GetTasksByStatus(WorkStatusFilter status)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workQueryService.GetTasksByStatusAsync(userId, status));
        }

        private async Task<IActionResult> GetTaskCountByStatus(WorkStatusFilter status)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _workQueryService.GetTaskCountByStatusAsync(userId, status));
        }

        private async Task<IActionResult> GetTaskCountByStatusForEmail(string email, WorkStatusFilter status)
        {
            return this.ToActionResult(await _workQueryService.GetTaskCountByStatusForEmailAsync(email, status));
        }
    }
}
