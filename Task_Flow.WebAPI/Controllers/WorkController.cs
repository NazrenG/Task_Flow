using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        private readonly ITaskService taskService;
        private readonly IUserService userService;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IProjectService projectService;
        private readonly MailService mailService;
        private readonly INotificationService notificationService;
        private readonly IHubContext<ConnectionHub> _context;
        private readonly TaskFlowDbContext _dbContext;
        private readonly IProjectActivityService _projectActivityService;
        private readonly IRequestNotificationService _requestNotificationService;
        private readonly INotificationSettingService _notificationSettingService;
        private readonly IGitHubService _gitHubService;

        public WorkController(ITaskService taskService, IUserService userService, UserManager<CustomUser> userManager, IProjectService projectService, MailService mailService, INotificationService notificationService, IHubContext<ConnectionHub> context, TaskFlowDbContext dbContext, IProjectActivityService projectActivityService, IRequestNotificationService requestNotificationService, INotificationSettingService notificationSettingService, IGitHubService gitHubService)
        {
            this.taskService = taskService;
            this.userService = userService;
            _userManager = userManager;
            this.projectService = projectService;
            this.mailService = mailService;
            this.notificationService = notificationService;
            _context = context;
            _dbContext = dbContext;
            _projectActivityService = projectActivityService;
            _requestNotificationService = requestNotificationService;
            _notificationSettingService = notificationSettingService;
           _gitHubService = gitHubService;
        }




        // GET: api/<WorkController>
        [Authorize]
        [HttpGet("UserTasks")]
        public async Task<IActionResult> GetUserTasks()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var list = await taskService.GetTasks(userId);
            var items = list.Select(p =>
            {
                return new WorkDto
                {
                    Id = p.Id,
                    CreatedById = userId,
                    Description = p.Description,
                    Deadline = p.Deadline,
                    Priority = p.Priority,
                    Status = p.Status,
                    Title = p.Title,
                    ProjectId = p.ProjectId,
                    ProjectName = p.Project?.Title,
                    StartDate = p.StartTime,
                    Color = p.Color,
                    CanbanColumnId=p.CanbanColumnId
                };
            }).ToList();
            return Ok(items);
        }
        [HttpGet("UserProfileTask/{email}")]
        public async Task<IActionResult> GetUserProfileTask(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var list = await taskService.GetTasks(user.Id);
            var items = list.Select(p =>
            {
                return new WorkDto
                {

                    CreatedById = user.Id,
                    Description = p.Description,
                    Deadline = p.Deadline,
                    Priority = p.Priority,
                    Status = p.Status,
                    Title = p.Title,
                    ProjectId = p.ProjectId,
                    ProjectName = p.Project?.Title,
                    StartDate = p.StartTime,
                    CanbanColumnId = p.CanbanColumnId
                };
            }).ToList();
            return Ok(items);
        }

        // GET api/<WorkController>/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var item = await taskService.GetTaskById(id);
            if (item == null)
            {
                return NotFound();
            }
            var work = new WorkDto
            {

                CreatedById = userId,
                Description = item.Description,
                Deadline = item.Deadline,
                Priority = item.Priority,
                Status = item.Status,
                Title = item.Title,
                ProjectId = item.ProjectId,
                ProjectName = item.Project?.Title,
                StartDate = item.StartTime,
                Color = item.Color,
                CanbanColumnId = item.CanbanColumnId,

            };
            return Ok(work);
        }

        [Authorize]
        [HttpGet("FullWorkDetail/{id}")]
        public async Task<IActionResult> GetFullWorkData(int id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var item = await taskService.GetTaskById(id);
            var project=await projectService.GetProjectById(item.ProjectId);
            var createdBy = await userService.GetUserById(item.CreatedById);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                Description = item.Description,
                Deadline = item.Deadline,
                Priority = item.Priority,
                Status = item.Status,
                Title = item.Title,
                StartDate = item.StartTime,
                Color = item.Color,
                CanbanColumnId = item.CanbanColumnId,
                gitHubBranchName = item.GitHubBranchName, // Branch adı
                project = new
                {
                    id =item.ProjectId,
                    title =project.Title,
                    gitHubRepositoryUrl = project.GitHubRepositoryUrl, // Repo URL
                    gitHubRepositoryName = project.GitHubRepositoryName
                },
                createdBy = new
                {
                    id = createdBy.Id,
                    username = createdBy.UserName,
                    firstname = createdBy.Firstname,
                    lastname = createdBy.Lastname
                }

            });
        }

        //// PUT api/<WorkController>/5
        [Authorize]
        [HttpPut("EditedProjectTask/{id}")]
        public async Task<IActionResult> PutProjectTaskByUser(int id, [FromBody] WorkDto value)
        {

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var item = await taskService.GetTaskById(id);

            if (item == null)
            {
                return NotFound();
            }
            item.Priority = value.Priority;
            item.Status = value.Status;
            await taskService.Update(item);
            await _projectActivityService.Add(new ProjectActivity
            {
                UserId = userId,
                ProjectId = value.ProjectId,
                Text = $"Task updated successfully. New task name: {value.Title}"
            });


            await _context.Clients.User(userId).SendAsync("OnHoldTaskCount");
            await _context.Clients.User(userId).SendAsync("RunningTaskCount");
            await _context.Clients.User(userId).SendAsync("CompletedTaskCount");
            return Ok(new { message = "update succesfuly" });
        }
        //canban icinde tasklari edit etmek
        [Authorize]
        [HttpPut("EditedProjectForPmTask/{id}")]
        public async Task<IActionResult> PutProjectForPmTask(int id, [FromBody] WorkDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var item = await taskService.GetTaskById(id);
            var project = await projectService.GetProjectById(value.ProjectId);
            var member = await userService.GetUserById(value.CreatedById);

            if (item == null)
            {
                return NotFound();
            }

            if (project.CreatedById != userId) return BadRequest(new { message = "You are not access edit task" });

            item.Title = value.Title;
            item.Description = value.Description;
            item.Deadline = value.Deadline;
            item.Color = value.Color;
            item.CreatedById = value.CreatedById;
            item.StartTime = value.StartDate;

            item.Priority = value.Priority;
            item.Status = value.Status;
            item.CanbanColumnId = value.CanbanColumnId;
            await taskService.Update(item);

            try
            {        //userin task listi ucun kanbandan gelen task  
                await _context.Clients.User(member.Id).SendAsync("UserTaskList");
                await _context.Clients.User(value.CreatedById).SendAsync("RunningTaskCount");
                await _context.Clients.User(value.CreatedById).SendAsync("CompletedTaskCount");
                await _context.Clients.User(value.CreatedById).SendAsync("OnHoldTaskCount");
                //canban ucun signalr 
                await _context.Clients.User(userId).SendAsync("CanbanTaskUpdated");
                await _context.Clients.User(item.CreatedById).SendAsync("CanbanTaskUpdated");
                await _context.Clients.User(member.Id).SendAsync("DashboardCalendarNotificationCount");

                //project ve view detail sehifesindeki task list
                await _context.Clients.User(value.CreatedById).SendAsync("ProjectsTaskList");
                await _context.Clients.User(value.CreatedById).SendAsync("ProjectDetailTaskList");

                //view profil sehifesindeki task list 
                await _context.Clients.User(value.CreatedById).SendAsync("UserProfileTask");

                //dashboard-da current project
                await _context.Clients.User(member.Id).SendAsync("DashboardReceiveProject");

                //project activity log signalr detail sehifesi
                await _context.Clients.User(member.Id).SendAsync("ProjectRecentActivityInDetail");
                await _context.Clients.User(userId).SendAsync("ProjectRecentActivityInDetail");
                //project activity log signalr project sehifesi
                await _context.Clients.User(member.Id).SendAsync("ProjectsRecentActivity");
                await _context.Clients.User(userId).SendAsync("ProjectsRecentActivity");
                // request getsin taski edit olan sexse
                var request = new RequestNotification
                {
                    IsAccepted = false,
                    ReceiverId = member.Id,
                    SenderId = userId,
                    NotificationType = "ProjectRequest",
                    ProjectName = project.Title,
                    SentDate = DateTime.UtcNow,
                    Text = $"Your task edit by {project.CreatedBy?.Firstname} {project.CreatedBy?.Lastname} in the project named {project.Title} "
                };
                await _requestNotificationService.Add(request);
                await _context.Clients.User(member.Id).SendAsync("RequestList2");
                await _context.Clients.User(member.Id).SendAsync("RequestCount");
                await _context.Clients.User(member.Id).SendAsync("RequestList");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR error: {ex.Message}");

            }
            //mail getsin taski edit olan sexse
            mailService.SendEmail(member.Email, $"Your task edit by {project.CreatedBy?.Firstname} {project.CreatedBy?.Lastname} in the project named {project.Title} ");



            await _projectActivityService.Add(new ProjectActivity
            {
                UserId = userId,
                ProjectId = value.ProjectId,
                Text = $"The task named '{value.Title}' has been successfully updated for {member.Firstname} {member.Lastname}.",
            });


            return Ok(new { message = "update succesfuly" });
        }

        [Authorize]
        [HttpGet("UserTasksCount")]
        public async Task<IActionResult> GetUserTaskCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var item = await taskService.GetTasks(userId);

            return Ok(item.Count());
        }

        [HttpGet("UserTasksCountForEmail/{email}")]
        public async Task<IActionResult> GetUserTaskCountForEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            var item = await taskService.GetTasks(user.Id);

            return Ok(item.Count());
        }

        // POST api/<WorkController>
        [Authorize]
        [HttpPost("NewTask")]
        public async Task<IActionResult> Post([FromBody] WorkDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var member = await userService.GetUserById(value.CreatedById);
            var project = await projectService.GetProjectById(value.ProjectId);
            var projectName = await projectService.GetProjectNameById(value.ProjectId);
            if (project == null) return NotFound("Project not found");
            var projectCreater = await projectService.GetProjectById(value.ProjectId);



            if (member == null || string.IsNullOrEmpty(member.GitHubAccessToken))
                return BadRequest("İstifadəçi GitHub hesabını qoşmalıdır");

            // Generate branch name from task title
            var branchName = $"task/{value.Title.ToLower().Replace(" ", "-")}";

            // Create branch using ASSIGNED USER's token (onlar öz branch-larını yaradır)
            var branchCreated = await _gitHubService.CreateBranch(
                member.GitHubAccessToken,
                project.CreatedBy.GitHubUsername,
                project.GitHubRepositoryName,
                branchName
            );

            if (!branchCreated)
                return StatusCode(500, "Branch yaradıla bilmədi");
            //  if (projectCreater.CreatedById != userId) return BadRequest("You do not have permission to update tasks in this project.");
            var item = new Work
            {
                CreatedById = value.CreatedById,
                Description = value.Description,
                Deadline = value.Deadline,
                Priority = value.Priority,
                Status = value.Status,
                Title = value.Title,
                Color = value.Color,
                ProjectId = value.ProjectId,
                CanbanColumnId = value.CanbanColumnId,
                GitHubBranchName=branchName,
                SprintId=value.SprintId,
            };
            await taskService.Add(item);
            try
            {        //userin task listi ucun kanbandan gelen task  
                await _context.Clients.User(member.Id).SendAsync("UserTaskList");
                await _context.Clients.User(value.CreatedById).SendAsync("TaskTotalCount");

                await _context.Clients.User(value.CreatedById).SendAsync("OnHoldTaskCount");
                //canban ucun signalr 
                await _context.Clients.User(userId).SendAsync("CanbanTaskUpdated");
                await _context.Clients.User(item.CreatedById).SendAsync("CanbanTaskUpdated");
                await _context.Clients.User(member.Id).SendAsync("DashboardCalendarNotificationCount");

                //project ve view detail sehifesindeki task list
                await _context.Clients.User(value.CreatedById).SendAsync("ProjectsTaskList");
                await _context.Clients.User(value.CreatedById).SendAsync("ProjectDetailTaskList");

                //view profil sehifesindeki task list 
                await _context.Clients.User(value.CreatedById).SendAsync("UserProfileTask");

                //dashboard-da current project
                await _context.Clients.User(member.Id).SendAsync("DashboardReceiveProject");
                //backlog
                await _context.Clients.All.SendAsync("UpdateBacklogTask");



            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR error: {ex.Message}");

            }

            await _requestNotificationService.Add(new RequestNotification
            {
                ReceiverId = value.CreatedById,
                SenderId = userId,
                Text = $"You have a new task({value.Title}) in the project named {projectCreater.Title}",
                IsAccepted = false,
                NotificationType = "ProjectRequest",
                ProjectName = projectName

            });
            //notification list project taski ucun 
            await _context.Clients.User(member.Id).SendAsync("RequestList2");
            await _context.Clients.User(member.Id).SendAsync("RequestCount");
            await _context.Clients.User(member.Id).SendAsync("RequestList");


            await _projectActivityService.Add(new ProjectActivity
            {
                UserId = userId,
                ProjectId = value.ProjectId,
                Text = $"A new task named '{value.Title}' has been created for {member.Firstname} {member.Lastname}.",
            });
            //project activity log signalr detail sehifesi
            await _context.Clients.User(member.Id).SendAsync("ProjectRecentActivityInDetail");
            await _context.Clients.User(userId).SendAsync("ProjectRecentActivityInDetail");
            //project activity log signalr project sehifesi
            await _context.Clients.User(member.Id).SendAsync("ProjectsRecentActivity");
            await _context.Clients.User(userId).SendAsync("ProjectsRecentActivity");

            //yeni task yaradilanda eger icaze varsa maile mesaj getsin
            var notificationSetting = await _notificationSettingService.GetNotificationSetting(userId);
          
            if (notificationSetting!=null && notificationSetting.NewTaskWithInProject)
            {
                mailService.SendEmail(member.Email, $"Hi,{member.Firstname} {member.Lastname}.You have a new task in the project named {project} ");

            }

            return Ok(item);
        }



        // DELETE api/<WorkController>/5
        [Authorize]
        [HttpDelete("DeleteProjectTask/{taskId}")]
        public async Task<IActionResult> Delete(int taskId, [FromQuery] int projectId)
        {
            var item = await taskService.GetTaskById(taskId);
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var project = await projectService.GetProjectById(projectId);
            if (project.CreatedById != userId) return BadRequest("You do not have permission to delete tasks in this project.");
            if (item == null)
            {
                return NotFound();
            }
            await taskService.Delete(item);


            try
            {        //userin task listi ucun kanbandan gelen task  
                await _context.Clients.User(item.CreatedById).SendAsync("UserTaskList");
                await _context.Clients.User(item.CreatedById).SendAsync("TaskTotalCount");
                await _context.Clients.User(item.CreatedById).SendAsync("RunningTaskCount");
                await _context.Clients.User(item.CreatedById).SendAsync("CompletedTaskCount");
                await _context.Clients.User(item.CreatedById).SendAsync("OnHoldTaskCount");
                //canban ucun signalr
                await _context.Clients.User(userId).SendAsync("CanbanTaskUpdated");
                await _context.Clients.User(item.CreatedById).SendAsync("CanbanTaskUpdated");

                //project ve view detail sehifesindeki task list
                await _context.Clients.User(item.CreatedById).SendAsync("ProjectsTaskList");
                await _context.Clients.User(item.CreatedById).SendAsync("ProjectDetailTaskList");

                //view profil sehifesindeki task list 
                await _context.Clients.User(item.CreatedById).SendAsync("UserProfileTask");

                //dashboard-da current project
                await _context.Clients.User(item.CreatedById).SendAsync("DashboardReceiveProject");
                //project activity log signalr detail sehifesi
                await _context.Clients.User(item.CreatedById).SendAsync("ProjectRecentActivityInDetail");
                await _context.Clients.User(userId).SendAsync("ProjectRecentActivityInDetail");
                //project activity log signalr project sehifesi
                await _context.Clients.User(item.CreatedById).SendAsync("ProjectsRecentActivity");
                await _context.Clients.User(userId).SendAsync("ProjectsRecentActivity");

        

            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR error: {ex.Message}");

            }

            await _projectActivityService.Add(new ProjectActivity
            {
                UserId = userId,
                ProjectId = projectId,
                Text = $"{item.CreatedBy?.Firstname} {item.CreatedBy?.Lastname}`s task delete successfully. New task name:{item.Title} "
            });
            await _requestNotificationService.Add(new RequestNotification
            {
                ReceiverId = item.CreatedById,
                SenderId = userId,
                Text = $"The task '{item.Title}' has been deleted by the project manager in the project '{project.Title}'.",
                IsAccepted = false,
                NotificationType = "ProjectRequest",
                ProjectName = project.Title,

            });
            //notification list project taski ucun 
            await _context.Clients.User(item.CreatedById).SendAsync("RequestList2");
            await _context.Clients.User(item.CreatedById).SendAsync("RequestCount");
            await _context.Clients.User(item.CreatedById).SendAsync("RequestList");

            //mail getsin taski silinen sexse
            mailService.SendEmail(item.CreatedBy?.Email, $"Your task'{item.Title}' has been deleted by the project manager in the project '{project.Title}' ");




            return Ok(new { message = "delete succesful" });
        }
        [Authorize]
        [HttpGet("DailyTask")]
        public async Task<IActionResult> GetDailyTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tasks = await taskService.GetTasks(userId);
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var todayTasks = tasks.Where(d => d.Deadline.Date == DateTime.Now.Date).OrderBy(t => t.StartTime).ToList();
            return Ok(todayTasks);

        }

        [Authorize]
        [HttpGet("ToDoTask")]
        public async Task<IActionResult> GetToDoTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var tasks = await taskService.GetToDoTask(userId);
            return Ok(tasks);

        }
        [Authorize]
        [HttpGet("ToDoTaskCount")]
        public async Task<IActionResult> GetToDoTaskCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var tasks = await taskService.GetToDoTask(userId);
            return Ok(tasks.Count);

        }

        [HttpGet("ToDoTaskCountForMail/{email}")]
        public async Task<IActionResult> GetToDoTaskCountForMail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }
            var tasks = await taskService.GetToDoTask(user.Id);
            return Ok(tasks.Count);

        }


        [Authorize]
        [HttpGet("InProgressTask")]
        public async Task<IActionResult> GetInProgressTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var tasks = await taskService.GetInProgressTask(userId);
            return Ok(tasks);

        }
        [Authorize]
        [HttpGet("InProgressTaskCount")]
        public async Task<IActionResult> GetInProgressTaskCountForEmail()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var tasks = await taskService.GetInProgressTask(userId);
            return Ok(tasks.Count);

        }
        [HttpGet("InProgressTaskCountForEmail/{email}")]
        public async Task<IActionResult> GetInProgressTaskCount(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }
            var tasks = await taskService.GetInProgressTask(user.Id);
            return Ok(tasks.Count);

        }

        [Authorize]
        [HttpGet("DoneTask")]
        public async Task<IActionResult> GetDoneTask()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var tasks = await taskService.GetDoneTask(userId);
            return Ok(tasks);

        }
        [Authorize]
        [HttpGet("DoneTaskCount")]
        public async Task<IActionResult> GetDoneTaskCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }
            var tasks = await taskService.GetDoneTask(userId);
            return Ok(tasks.Count);

        }
        [HttpGet("DoneTaskCountForEmail/{email}")]
        public async Task<IActionResult> GetDoneTaskCountForEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            var tasks = await taskService.GetDoneTask(user.Id);
            return Ok(tasks.Count);

        }

        [Authorize]
        [HttpGet("UserWorks")]
        public async Task<IActionResult> GetUserWorks()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var works = await _dbContext.Works
                .Include(w => w.Project)
                .Include(w => w.CreatedBy)
                .Where(w => w.Project.CreatedById == userId)
                .ToListAsync();

            if (works == null || !works.Any())
            {
                return NotFound("No works found for the current user.");
            }

            var dtoList = works.Select(work => new WorkDetailsDto
            {
                TaskId = work.Id,
                ProjectId = work.ProjectId,
                ProjectName = work.Project?.Title,
                MemberName = $"{work.CreatedBy?.Firstname} {work.CreatedBy?.Lastname}",
                MemberImage = work.CreatedBy?.Image,
                MemberMail = work.CreatedBy?.Email,
                TaskTitle = work.Title,
                StartTime = work.StartTime ?? DateTime.Now,
                Deadline = work.Deadline,
                Status = work.Status,
                Priority = work.Priority
            }).ToList();

            return Ok(dtoList);
        }

        [Authorize]
        [HttpGet("ProjectWorks/{projectId}")]
        public async Task<IActionResult> GetProjectWorks(int projectId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var works = await taskService.GetByProjectId(projectId);


            if (works == null || !works.Any())
            {
                works = new List<Work>();
            }
            var dtoList = works.Select(work => new WorkDetailsDto
            {
                TaskId = work.Id,
                ProjectId = work.ProjectId,
                ProjectName = work.Project?.Title,
                MemberName = $"{work.CreatedBy?.Firstname} {work.CreatedBy?.Lastname}",
                MemberImage = work.CreatedBy?.Image,
                MemberMail = work.CreatedBy?.Email,
                TaskTitle = work.Title,
                StartTime = work.StartTime ?? DateTime.Now,
                Deadline = work.Deadline,
                Status = work.Status,
                Priority = work.Priority
            }).ToList();

            return Ok(dtoList);
        }

        [Authorize]
        [HttpGet("Backlogs/{projectId}")]
        public async Task<IActionResult> GetBacklogs(int projectId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "User not authenticated." });
            }

            var works = await taskService.GetBacklogs(projectId);


            if (works == null || !works.Any())
            {
                works = new List<Work>();
            }

            var dtoList = works.Select(work => new WorkDetailsDto
            {
                TaskId = work.Id,
                ProjectId = work.ProjectId,
                ProjectName = work.Project?.Title,
                MemberName = $"{work.CreatedBy?.Firstname} {work.CreatedBy?.Lastname}",
                MemberImage = work.CreatedBy?.Image,
                MemberMail = work.CreatedBy?.Email,
                TaskTitle = work.Title,
                StartTime = work.StartTime ?? DateTime.Now,
                Deadline = work.Deadline,
                Status = work.Status,
                Priority = work.Priority
            }).ToList();

            return Ok(dtoList);
        }


    }
}