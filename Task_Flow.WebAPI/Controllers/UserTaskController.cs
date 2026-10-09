using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.UserTasks;
using Task_Flow.WebAPI.Services.Works;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskQueryService _userTaskQueryService;
        private readonly IUserTaskCommandService _userTaskCommandService;

        public UserTaskController(IUserTaskQueryService userTaskQueryService, IUserTaskCommandService userTaskCommandService)
        {
            _userTaskQueryService = userTaskQueryService;
            _userTaskCommandService = userTaskCommandService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        private IActionResult UserNotAuthenticated() => BadRequest(new { message = "User not authenticated." });

        [Authorize]
        [HttpGet("UserTasks")]
        public async Task<IActionResult> GetUserTasks()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _userTaskQueryService.GetUserTasksAsync(userId));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserTask(int id)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _userTaskQueryService.GetUserTaskAsync(id, userId));
        }

        [Authorize]
        [HttpPut("EditedTaskForCalendar/{id}")]
        public async Task<IActionResult> PutEditTaskForCalendar(int id, [FromBody] EditForCalendarDto value)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _userTaskCommandService.EditTaskForCalendarAsync(id, userId, value));
        }

        [Authorize]
        [HttpPut("EditedTask/{id}")]
        public async Task<IActionResult> PutEditTask(int id, [FromBody] WorkDto value)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _userTaskCommandService.EditTaskAsync(id, userId, value));
        }

        [Authorize]
        [HttpGet("UserTasksCount")]
        public async Task<IActionResult> GetUserTaskCount()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _userTaskQueryService.GetUserTaskCountAsync(userId));
        }

        [Authorize]
        [HttpPost("NewTask")]
        public async Task<IActionResult> Post([FromBody] WorkDto value)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _userTaskCommandService.CreateTaskAsync(userId, value));
        }

        [Authorize]
        [HttpDelete("DeleteUserTask/{taskId}")]
        public async Task<IActionResult> Delete(int taskId)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _userTaskCommandService.DeleteTaskAsync(taskId, userId));
        }

        [Authorize]
        [HttpGet("DailyTask")]
        public async Task<IActionResult> GetDailyTask()
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _userTaskQueryService.GetDailyTasksAsync(userId));
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

        private async Task<IActionResult> GetTasksByStatus(WorkStatusFilter status)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _userTaskQueryService.GetTasksByStatusAsync(userId, status));
        }

        private async Task<IActionResult> GetTaskCountByStatus(WorkStatusFilter status)
        {
            var userId = CurrentUserId;
            if (userId == null) return UserNotAuthenticated();

            return this.ToActionResult(await _userTaskQueryService.GetTaskCountByStatusAsync(userId, status));
        }

        private async Task<IActionResult> GetTaskCountByStatusForEmail(string email, WorkStatusFilter status)
        {
            return this.ToActionResult(await _userTaskQueryService.GetTaskCountByStatusForEmailAsync(email, status));
        }
    }
}
