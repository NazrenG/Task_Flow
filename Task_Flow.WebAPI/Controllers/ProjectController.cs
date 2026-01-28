using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
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
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly TaskFlowDbContext _context;
        private readonly IUserService _userService;
        private readonly ITaskService _taskService;
        private readonly ITeamMemberService _teamMemberService;
        private readonly IProjectActivityService _projectActivity;
        private readonly IHubContext<ConnectionHub> _hub;
        private readonly IRequestNotificationService _requestNotificationService;
        private readonly Business.Cocrete.MailService mailService;
        private readonly IPremiumUserService _premiumUserService;

        public ProjectController(IProjectService projectService, TaskFlowDbContext context, IUserService userService, ITaskService taskService, ITeamMemberService teamMemberService, IProjectActivityService projectActivity, IHubContext<ConnectionHub> hub, IRequestNotificationService requestNotificationService, Business.Cocrete.MailService mailService, IPremiumUserService premiumUserService)
        {
            _projectService = projectService;
            _context = context;
            _userService = userService;
            _taskService = taskService;
            _teamMemberService = teamMemberService;
            _projectActivity = projectActivity;
            _hub = hub;
            _requestNotificationService = requestNotificationService;
            this.mailService = mailService;
            _premiumUserService = premiumUserService;
        }

        [HttpGet("ProjectTitle/{projectId}")]
        public async Task<IActionResult> GetProjectTitle(int projectId)
        {
            var project=await _projectService.GetProjectById(projectId);
            if(project==null) return NotFound();
            return Ok(new { Title = project.Title ,Color=project.Color});
        }


        [Authorize]
        [HttpGet("ExtendedProjectList")]
        public async Task<IActionResult> GetExtendedProjectList()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token or user not found.");
            }

            var item = await _userService.GetUserById(userId);
            if (item == null)
            {
                return NotFound("User not found.");
            }

            var projectList = await _projectService.GetProjects(userId);
            var list = projectList.Select(p => new ExtendedProjectListDto
            {
                Id = p.Id,
                EndDate = p.EndDate,
                StartDate = p.StartDate,
                Title = p.Title,
                Deadline=p.EndDate,
                TotalTask = p.TaskForUsers.Count,
                CompletedTask = p.TaskForUsers.Count(t => t.Status == "done"),
                ParticipantsPath = p.TeamMembers!
                            .Select(tm => tm.User.Image)!
                            .ToList(),
                Color = p.Color,

            }).ToList();

            return Ok(list);
        }




        [Authorize]
        [HttpGet("UserProjectCount")]
        public async Task<IActionResult> GetUserProjectCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            // var item = await _userService.GetUserById(userId);
            var count = await _projectService.GetUserProjectCount(userId);

            return Ok(count);
        }
        [Authorize]
        [HttpPost("ProjectWithTitle")]
        public async Task<IActionResult> GetProjectWithTitle([FromBody] string title)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not authorized." });
                }

                if (string.IsNullOrEmpty(title))
                {
                    return BadRequest(new { message = "Title is required." });
                }
                var project = await _projectService.GetProjectByName(userId, title);
                if (project == null)
                {
                    return NotFound(new { message = "Project not found." });
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }

        [HttpGet("{id}")] ///Sevgi
        public async Task<IActionResult> GetProject(int id)
        {
            var item = await _projectService.GetProjectById(id);
            if (item == null)
            {
                return NotFound();
            }

            var project = new ProjectDto
            {
                Owner = item.CreatedBy?.UserName,
                OwnerMail=item.CreatedBy?.Email,    
                IsCompleted = item.IsCompleted,
                Description = item.Description,
                Title = item.Title,
                Color = item.Color,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                Status = item.Status,  
               
            };

            var teamMembers = await _teamMemberService.GetTaskMemberListById(id);
            var memberUsernames = new List<string>();
            var membersPath=new List<string>(); 

            foreach (var teamMember in teamMembers)
            {
                var user = await _userService.GetUserById(teamMember.UserId);
                if (user != null)
                {
                    memberUsernames.Add(user.UserName);
                    membersPath.Add(user.Image); 
                }
            }

            project.Members = memberUsernames;
           project.MembersPath = membersPath;

            return Ok(project);
        }




        // Pryektin icindeki tasklar CANBAN 

        [HttpGet("ProjectTaskCanban/{projectId}")]
        public async Task<IActionResult> Get(int projectId)
        {
            var item = await _projectService.GetProjectById(projectId);
            if (item == null)
            {
                return NotFound();
            }
            var projectTasks = await _taskService.GetByProjectId(projectId);
             
            var items = projectTasks.Select(p =>
            {
                return new CanbanTaskDto
                {
                    Id = p.Id,
                    CreatedById = p.CreatedById,
                    Description = p.Description,
                    Deadline = p.Deadline,
                    Priority = p.Priority,
                    Status = p.Status,
                    Title = p.Title,
                    StartDate = p.StartTime,
                    Color = p.Color,  
                 

                    ParticipantPath = p.CreatedBy?.Image ?? "default-path.png",  
                    ParticipantName = p.CreatedBy != null
            ? $"{p.CreatedBy.Firstname} {p.CreatedBy.Lastname}"
            : "Unknown Participant",
                    ParticipantEmail = p.CreatedBy?.Email ?? "unknown@example.com",
                };
            }).ToList();
            return Ok(items);


        }
        //canbanda tasklarin statusunu deyisdirmek 
        [Authorize]
        [HttpPut("UpdateTaskStatus/{id}")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto updateTaskStatusDto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var task = await _taskService.GetTaskById(id);
            
            if (task == null)
                return NotFound("Task not found.");

            var project = await _projectService.GetProjectById(task.ProjectId);

            //proyekt sahibi deyise bilsin
            if (project.CreatedById != userId)
                return BadRequest("You do not have permission to update tasks in this project.");

            task.Status = updateTaskStatusDto.NewStatus;
            await _taskService.Update(task);


            try
            {        //userin task listi ucun kanbandan gelen task  
                await _hub.Clients.User(task.CreatedById).SendAsync("UserTaskList");
                await _hub.Clients.User(task.CreatedById).SendAsync("RunningTaskCount");
                await _hub.Clients.User(task.CreatedById).SendAsync("CompletedTaskCount");
                await _hub.Clients.User(task.CreatedById).SendAsync("OnHoldTaskCount");
                await _hub.Clients.User(task.CreatedById).SendAsync("TaskTotalCount");//task siline biler

                //canban ucun signalr 
                await _hub.Clients.User(userId).SendAsync("CanbanTaskUpdated");
                await _hub.Clients.User(task.CreatedById).SendAsync("CanbanTaskUpdated");
                await _hub.Clients.User(task.CreatedById).SendAsync("DashboardCalendarNotificationCount");
                //dashboard-da current project
                await _hub.Clients.User(task.CreatedById).SendAsync("DashboardReceiveProject");
            

                //project ve view detail sehifesindeki task list
    await _hub.Clients.User(task.CreatedById).SendAsync("ProjectsTaskList");
    await _hub.Clients.User(task.CreatedById).SendAsync("ProjectDetailTaskList");

                //view profil sehifesindeki task list 
    await _hub.Clients.User(task.CreatedById).SendAsync("UserProfileTask");
                //project activity log signalr detail sehifesi
                await _hub.Clients.User(task.CreatedById).SendAsync("ProjectRecentActivityInDetail");
                await _hub.Clients.User(userId).SendAsync("ProjectRecentActivityInDetail");
                //project activity log signalr project sehifesi
                await _hub.Clients.User(task.CreatedById).SendAsync("ProjectsRecentActivity");
                await _hub.Clients.User(userId).SendAsync("ProjectsRecentActivity");
                // request getsin taski edit olan sexse
                var request = new RequestNotification
                {
                    IsAccepted = false,
                    ReceiverId = task.CreatedById,
                    SenderId = userId,
                    NotificationType = "ProjectRequest",
                    ProjectName = project.Title,
                    SentDate = DateTime.UtcNow,
                    Text = $"Your task edit by {project.CreatedBy?.Firstname} {project.CreatedBy?.Lastname} in the project named {project.Title} "
                };
                await _requestNotificationService.Add(request);
                await _hub.Clients.User(task.CreatedById).SendAsync("RequestList2");
                await _hub.Clients.User(task.CreatedById).SendAsync("RequestCount");
                await _hub.Clients.User(task.CreatedById).SendAsync("RequestList");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR error: {ex.Message}");

            }
            //mail getsin taski edit olan sexse
            mailService.SendEmail(task.CreatedBy?.Email, $"Your task edit in the project named {project} ");


            await _projectActivity.Add(new ProjectActivity
            {
                UserId = userId,
                ProjectId = task.ProjectId,
                Text = $"The task named '{task.Title}' has been successfully updated for {task.CreatedBy?.Firstname} {task.CreatedBy?.Lastname}.",
            });

            return Ok("Task status updated successfully.");
        }

        [Authorize]
        [HttpGet("AllProjectsUserOwn")]
        public async Task<IActionResult> GetUserOwnProjects()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var list = await _projectService.GetProjects(userId);

            return Ok(list);

        }

        [Authorize]
        [HttpGet("UserAddedProjects")]
        public async Task<IActionResult> GetUserAddedProjects()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var list = await _teamMemberService.GetProjectListByUserIdAsync(userId);
            var projects = new List<Project>();
            foreach (var item in list)
            {
                projects.Add(await _projectService.GetProjectById(item.ProjectId));
            }
            return Ok(projects);

        }

        [HttpGet("ProjectTaskCount/{id}")]
        public async Task<IActionResult> GetProjectTaskCount(int id)
        {

            var item = await _context.Projects
       .Include(p => p.TaskForUsers)
       .FirstOrDefaultAsync(p => p.Id == id);
            //   var item = await _projectService.GetProjectById(id);
            if (item == null)
            {
                return NotFound();
            }
            var count = item.TaskForUsers?.Count;

            return Ok(count);

        }

        // POST api/<ProjectController>
        [Authorize]///Sevgi
        [HttpPost]///Sevgi
        public async Task<IActionResult> Post([FromBody] ProjectDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var item = new Project
            {
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow,
                StartDate = value.StartDate,
                EndDate = value.EndDate,
                Status = value.Status,
                Description = value.Description,
                IsCompleted = value.IsCompleted,
                Title = value.Title,
                Color = value.Color,
            };
            var result = await _premiumUserService.IsUserAllowedToCreateProjectAsync(userId, item);
            if (!result.Allowed) { return Ok(new { message = result.Message, allowed = false }); }
            await _projectService.Add(item);


            await _projectActivity.Add(new ProjectActivity { UserId = userId, ProjectId = item.Id, Text = "created a new Project named: " + item.Title });
            //var count=await _projectService.
            //await _hub.Clients.All.SendAsync("ProjectCountUpdate");
            await _hub.Clients.User(userId).SendAsync("ReceiveProjectUpdate");
            await _hub.Clients.User(userId).SendAsync("RecieveInProgressUpdate");
            await _hub.Clients.User(userId).SendAsync("UpdateTotalProjects");//s
            if (value.Status == "On Going")
                await _hub.Clients.User(userId).SendAsync("UpdateOnGoingProjects");
            else if (value.Status == "Pending")
                await _hub.Clients.User(userId).SendAsync("UpdatePendingProjects");
            else if (value.Status == "Completed")
                await _hub.Clients.User(userId).SendAsync("UpdateCompletedProjects");
            return Ok(new {item=item,allowed=true});
        }

        [Authorize]
        [HttpPut("Put/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] PutProjectDto dto)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = await _projectService.GetProjectById(id);
            if (item == null)
            {
                return NotFound();
            }
            item.Title = dto.Title;
            item.Color = dto.Color;
            item.Description = dto.Description;
            item.StartDate = dto.StartDate;
            item.EndDate = dto.EndDate;
            await _projectService.Update(item);
            await _projectActivity.Add(new ProjectActivity { UserId = userId, ProjectId = id, Text = "Changed Project Title to: " + item.Title });
            await _hub.Clients.User(userId).SendAsync("ReceiveProjectUpdate");
            await _hub.Clients.User(userId).SendAsync("RecieveInProgressUpdate");
           
    
        await _hub.Clients.All.SendAsync("ReceiveProjectUpdateDashboard");
    
            return Ok();

        }

        // PUT api/<ProjectController>/5
        [Authorize]
        [HttpPut("ChangeTitle/{id}")]
        public async Task<IActionResult> PutTitle(int id, [FromBody] string value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var item = await _projectService.GetProjectById(id);

            if (item == null)
            {
                return NotFound();
            }
            item.Title = value;
            await _projectService.Update(item);
            await _projectActivity.Add(new ProjectActivity { UserId = userId, ProjectId = id, Text = "Changed Project Title to: " + item.Title });
            return Ok();
        }

        [Authorize]
        [HttpPut("ChangeDescription/{id}")]
        public async Task<IActionResult> PutDescription(int id, [FromBody] string value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = await _projectService.GetProjectById(id);

            if (item == null)
            {
                return NotFound();
            }
            item.Description = value;
            await _projectService.Update(item);
            await _projectActivity.Add(new ProjectActivity { UserId = userId, ProjectId = id, Text = "Changed Project Description" });
            return Ok();
        }
        [Authorize]
        [HttpPut("ChangeCompleted/{id}")]
        public async Task<IActionResult> PutCompleted(int id, [FromBody] bool value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = await _projectService.GetProjectById(id);

            if (item == null)
            {
                return NotFound();
            }
            item.IsCompleted = value;
            await _projectService.Update(item);
            await _projectActivity.Add(new ProjectActivity { UserId = userId, ProjectId = id, Text = "Project Completed" });
            return Ok();
        }
        // DELETE api/<ProjectController>/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = await _projectService.GetProjectById(id);
            if (item == null)
            {
                return NotFound();
            }
            await _projectActivity.Add(new ProjectActivity { UserId = userId, ProjectId = id, Text = "Project (" + item.Title + ") Deleted!" });
            await _projectService.Delete(item);
            await _hub.Clients.User(userId).SendAsync("RecieveInProgressUpdate");
            await _hub.Clients.User(userId).SendAsync("RequestList");
            return Ok(item);
        }


        [Authorize]
        [HttpGet("OnGoingProject")]
        public async Task<IActionResult> GetOnGoingProject()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
           
            var projects = await _projectService.GetOnGoingProject(userId);
            var list = projects.Select(p => new
            {
                Title = p.Title,
                EndDate = p.EndDate,
                StartDate = p.StartDate,
                MembersPath = p.TeamMembers!.Select(tm => tm.User?.Image)!
                        .ToList(),
                        Color= p.Color, 
            });
            return Ok(list);

        }
        [Authorize]
        [HttpGet("PendingProject")]
        public async Task<IActionResult> GetPendingProject()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var projects = await _projectService.GetPendingProject(userId);
            return Ok(projects);

        }
        [Authorize]
        [HttpGet("CompletedProject")]
        public async Task<IActionResult> GetCompletedProject()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var projects = await _projectService.GetCompletedTask(userId);
            return Ok(projects);

        }

        [Authorize]
        [HttpGet("OnGoingProjectCount")]
        public async Task<IActionResult> GetOnGoingProjectCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var projects = await _projectService.GetOnGoingProject(userId);
            return Ok(projects.Count == null ? 0 : projects.Count);

        }

        [Authorize]
        [HttpGet("PendingProjectCount")]
        public async Task<IActionResult> GetPendingProjectCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var projects = await _projectService.GetPendingProject(userId);
            return Ok(projects.Count == null ? 0 : projects.Count);

        }

        [Authorize]
        [HttpGet("CompletedTaskCount")]
        public async Task<IActionResult> GetCompletedTaskCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var projects = await _projectService.GetCompletedTask(userId);
            return Ok(projects.Count == null ? 0 : projects.Count);

        }

        [Authorize]
        [HttpGet("ProjectNames")]
        public async Task<IActionResult> GetProjectNames()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var projects = await _projectService.GetProjects(userId);
            var names = new List<string>();
            foreach (var project in projects)
            {
                names.Add(project.Title);
            }
            return Ok(new { Names = names });
        }

        [Authorize]

        [HttpPost("TasksDependingMonths")]///Sevgi

        public async Task<IActionResult> GetTasksForChart([FromBody] string projectName)
        {

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            List<string> months = new List<string>();
            DateTime currentDate = DateTime.Now;
            var completedTasks = new List<int>();
            var onGoingTasks = new List<int>();
            var year = DateTime.UtcNow.Year;

            var project = await _projectService.GetProjectByName(userId, projectName);

            for (int i = 0; i < 6; i++)
            {
                int monthIndex = (currentDate.Month - i + 12) % 12;
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthIndex + 1);
                var data = await _taskService.GetTaskSummaryByMonthAsync(project.Id, monthIndex, year);
                months.Insert(0, monthName);
                completedTasks.Add(data[0]);
                onGoingTasks.Add(data[1]);
                if (monthName == "January") year--;
            }
            completedTasks.Reverse();
            onGoingTasks.Reverse();



            return Ok(new { Complated = completedTasks, OnGoing = onGoingTasks });
        }

        [Authorize]//userin istirak etdiyi layiheler=> dashboardda
        [HttpGet("ProjectInvolved")]
        public async Task<IActionResult> ProjectInvolved()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var participantProjects = await _teamMemberService.GetProjectListByUserIdAsync(userId);

            var projectList = new List<ExtendedProjectListDto>();

            foreach (var item in participantProjects)
            {
                var project = await _projectService.GetProjectById(item.ProjectId);
                projectList.Add(new ExtendedProjectListDto
                {
                    Id = project.Id,
                    Title = project.Title,
                    TotalTask = project.TaskForUsers!.Count(),
                    CompletedTask = project.TaskForUsers!.Count(t => t.Status == "done"),
                    ParticipantsPath = project.TeamMembers!
                        .Select(tm => tm.User.Image)!
                        .ToList(),
                    Color = project.Color,
                });
            }
            return Ok(projectList);


        }



    }
}
