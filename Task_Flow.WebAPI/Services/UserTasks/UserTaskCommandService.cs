using Task_Flow.Business.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.UserTasks
{
    public class UserTaskCommandService : IUserTaskCommandService
    {
        private const string UpdateSuccessMessage = "update succesfuly";

        private readonly IUserTaskService _userTaskService;
        private readonly IUserTaskRealtimeNotifier _notifier;

        public UserTaskCommandService(IUserTaskService userTaskService, IUserTaskRealtimeNotifier notifier)
        {
            _userTaskService = userTaskService;
            _notifier = notifier;
        }

        public async Task<ServiceResult<UserTask>> CreateTaskAsync(string userId, WorkDto value)
        {
            var titleExists = await _userTaskService.CheckTaskName(value.Title!);
            if (titleExists)
            {
                return ServiceResult<UserTask>.BadRequest(new { message = "this title already has." });
            }

            var task = value.ToNewUserTask(userId);
            await _userTaskService.Add(task);
            await _notifier.NotifyTaskCreatedAsync(userId);

            return ServiceResult<UserTask>.Success(task);
        }

        public async Task<ServiceResult<object>> EditTaskAsync(int id, string userId, WorkDto value)
        {
            var task = await _userTaskService.GetById(id);

            task.ApplyEdit(value);
            await _userTaskService.Update(task);
            await _notifier.NotifyTaskEditedAsync(userId, task.CreatedById!);

            return ServiceResult<object>.Success(new { message = UpdateSuccessMessage });
        }

        public async Task<ServiceResult<object>> EditTaskForCalendarAsync(int id, string userId, EditForCalendarDto value)
        {
            var task = await _userTaskService.GetById(id);

            task.ApplyCalendarEdit(value);
            await _userTaskService.Update(task);
            await _notifier.NotifyCalendarTaskEditedAsync(userId);

            return ServiceResult<object>.Success(new { message = UpdateSuccessMessage });
        }

        public async Task<ServiceResult<object>> DeleteTaskAsync(int taskId, string userId)
        {
            var task = await _userTaskService.GetById(taskId);
            if (task == null)
            {
                return ServiceResult<object>.NotFound();
            }

            await _userTaskService.Delete(task);
            await _notifier.NotifyTaskDeletedAsync(userId, task.Status);

            return ServiceResult<object>.Success(new { message = "delete succesful" });
        }
    }
}
