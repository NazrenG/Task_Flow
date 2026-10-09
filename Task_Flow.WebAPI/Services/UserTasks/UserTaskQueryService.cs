using Microsoft.AspNetCore.Identity;
using Task_Flow.Business.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Results;
using Task_Flow.WebAPI.Services.Works;

namespace Task_Flow.WebAPI.Services.UserTasks
{
    public class UserTaskQueryService : IUserTaskQueryService
    {
        private readonly IUserTaskService _userTaskService;
        private readonly UserManager<CustomUser> _userManager;

        public UserTaskQueryService(IUserTaskService userTaskService, UserManager<CustomUser> userManager)
        {
            _userTaskService = userTaskService;
            _userManager = userManager;
        }

        public async Task<ServiceResult<List<WorkDto>>> GetUserTasksAsync(string userId)
        {
            var tasks = await _userTaskService.GetUserTasks(userId);
            return ServiceResult<List<WorkDto>>.Success(tasks.Select(t => t.ToWorkDto(userId)).ToList());
        }

        public async Task<ServiceResult<WorkDto>> GetUserTaskAsync(int id, string userId)
        {
            var task = await _userTaskService.GetById(id);
            return ServiceResult<WorkDto>.Success(task.ToSingleWorkDto(userId));
        }

        public async Task<ServiceResult<int>> GetUserTaskCountAsync(string userId)
        {
            var tasks = await _userTaskService.GetUserTasks(userId);
            return ServiceResult<int>.Success(tasks.Count);
        }

        public async Task<ServiceResult<List<UserTask>>> GetDailyTasksAsync(string userId)
        {
            var tasks = await _userTaskService.GetUserTasks(userId);
            var todayTasks = tasks
                .Where(t => t.Deadline.Date == DateTime.Now.Date)
                .OrderBy(t => t.StartTime)
                .ToList();

            return ServiceResult<List<UserTask>>.Success(todayTasks);
        }

        public async Task<ServiceResult<List<UserTask>>> GetTasksByStatusAsync(string userId, WorkStatusFilter status)
        {
            return ServiceResult<List<UserTask>>.Success(await LoadTasksByStatusAsync(userId, status));
        }

        public async Task<ServiceResult<int>> GetTaskCountByStatusAsync(string userId, WorkStatusFilter status)
        {
            var tasks = await LoadTasksByStatusAsync(userId, status);
            return ServiceResult<int>.Success(tasks.Count);
        }

        public async Task<ServiceResult<int>> GetTaskCountByStatusForEmailAsync(string email, WorkStatusFilter status)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ServiceResult<int>.BadRequest(new { message = "User not found." });
            }

            return await GetTaskCountByStatusAsync(user.Id, status);
        }

        private Task<List<UserTask>> LoadTasksByStatusAsync(string userId, WorkStatusFilter status)
        {
            return status switch
            {
                WorkStatusFilter.ToDo => _userTaskService.GetToDoTask(userId),
                WorkStatusFilter.InProgress => _userTaskService.GetInProgressTask(userId),
                WorkStatusFilter.Done => _userTaskService.GetDoneTask(userId),
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }
    }
}
