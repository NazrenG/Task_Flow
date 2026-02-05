using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamMemberController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ITeamMemberService _teamMemberService;
        private readonly IProjectService _projectService;
        private readonly MailService mailService;
        private readonly IHubContext<ConnectionHub> _hub;
        private readonly  IRequestNotificationService _requestNotificationService;
        private readonly INotificationSettingService _notificationSettingService;
        private readonly IGitHubService _gitHubService;
  private readonly IPremiumUserService _premiumUserService;
       
        public TeamMemberController(ITeamMemberService teamMemberService, IUserService userService, IProjectService projectService, IRequestNotificationService requestNotificationService, IHubContext<ConnectionHub> hub, INotificationSettingService notificationSettingService,MailService mailServicse,IPremiumUserService premiumUserService,IGitHubService gitHubService)
        {
            _userService = userService;
            _teamMemberService = teamMemberService;
            _projectService = projectService;
            this.mailService = mailServicse;
            _requestNotificationService = requestNotificationService;
            _hub = hub;

                _premiumUserService = premiumUserService;
            _notificationSettingService = notificationSettingService;
            _gitHubService = gitHubService;
        }

        [HttpGet("AllMember")]
        public async Task<IEnumerable<TeamMemberDto>> Get()
        {
           var items= await _teamMemberService.TeamMembers();
            var list = items.Select(t =>
            {
                return  new TeamMemberDto
                {
                    ProjectId=t.ProjectId,
                    UserId=t.UserId,

                };
            }); 
            return list;
        }
         
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMember(int id)
        {
         var item=await _teamMemberService.GetTaskMemberById(id);
            if (item == null)
            {
               return NotFound(item);
            }
            return Ok(new TeamMemberDto
            {
                ProjectId = item.ProjectId,
                UserId = item.UserId,
            });
        }

       
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TeamMemberDto value)
        {
            if (value == null)
            {
                return BadRequest();
            }
            var member = await _userService.GetUserById(value.UserId);

            if (string.IsNullOrEmpty(member.GitHubUsername))
                return BadRequest("İstifadəçi GitHub hesabını qoşmalıdır");
            var project = await _projectService.GetProjectById(value.ProjectId);

            // Add as collaborator on GitHub
            var success = await _gitHubService.AddCollaborator(
                project.CreatedBy.GitHubAccessToken,
                project.CreatedBy.GitHubUsername,
                project.GitHubRepositoryName,
                member.GitHubUsername
            );
            if (!success)
                return StatusCode(500, "GitHub-da əlavə edilə bilmədi");

            var item = new TeamMember
            {
                ProjectId = value.ProjectId,
                UserId = value.UserId,
                GitHubAccessGranted = true

            };
           await _teamMemberService.Add(item);
            return Ok(item);
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] string value)
        {
            var item = await _teamMemberService.GetTaskMemberById(id);
            if (item == null)
            {
                return NotFound();
            } 
            item.UserId = value; 
            await _teamMemberService.Update(item);
            return Ok();

        }

        //[Authorize]
        //[HttpGet("TeamMembersActivity")]
        //public async Task<IActionResult> GetTeamMemberAvtivity()
        //{
        //    var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var data = _teamMemberService.GetTaskMemberById();
        //}


        // DELETE api/<TeamMemberController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _teamMemberService.GetTaskMemberById(id);
            if (item == null)
            {
                return NotFound();
            }
            await _teamMemberService.Delete(item);
            return Ok();
        }

        [Authorize]
        [HttpPost("UpdateTeamMemberCollections")]///Sevgi
        public async Task<IActionResult> UpdateTeamMembersAsTeam([FromBody] TeamMemberCollectionDto dto)
        {
            if (dto == null || dto.Members == null || !dto.Members.Any())
            {
                return Ok(new { Message = "No member" });
            }

            try
            {
                var senderId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var sender = await _userService.GetUserById(senderId);
                var list = await _teamMemberService.GetTaskMemberListById(dto.ProjectId);
                await _teamMemberService.RemoveMembers(list);
                var result = await _premiumUserService.IsUserAllowedToAddTeammember(list.Count, senderId);
                if (!result.Allowed) return Ok(new { message = result.Message });

                foreach (var username in dto.Members)
                {
                    var user = await _userService.GetOneUSerByUsername(username);
                    var project = await _projectService.GetProjectNameById(dto.ProjectId);
                    //var isCheck = await _teamMemberService.GetTaskMemberById(user.Id);
                    if (user == null)
                    {
                        return NotFound(new { Message = $"User '{username}' not found." });
                    }

                    //var teamMember = new TeamMember
                    //{
                    //    ProjectId = dto.ProjectId,
                    //    UserId = user.Id,
                    //};
                    var request = new RequestNotification
                    {
                        IsAccepted = false, 
                        ReceiverId = user.Id,
                        SenderId = senderId,
                        NotificationType = "ProjectRequest",
                        ProjectName = project,
                        SentDate = DateTime.UtcNow,
                        Text = "Hi, I am " + sender.Firstname + " " + sender.Lastname + ". I want to invite you to my project named: " + project,
                    };
                    await _requestNotificationService.Add(request);
                    //notification list
                    await _hub.Clients.User(user.Id).SendAsync("RequestList2");
                    await _hub.Clients.User(user.Id).SendAsync("RequestCount");
                    await _hub.Clients.User(user.Id).SendAsync("RequestList");
                    //proyektde istirrak ucun egere icaze varsa mail gedir
                    var notificationSetting = await _notificationSettingService.GetNotificationSetting(user.Id);
                    if (notificationSetting.NewTaskWithInProject)
                    {
                        mailService.SendEmail(user.Email, $"Hi,{user.Firstname} {user.Lastname}.You have a new task in the project named {project} ");

                    }


                    //await _teamMemberService.Add(teamMember);
                }

                return Ok(new { Message = "Team members added successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while adding team members.", Details = ex.Message });
            }
        }
        [HttpGet("GetUsersByProject/{projectId}")]//nezrin
        public async Task<IActionResult> GetUsersByProject(int projectId)
        {
            var users = await _teamMemberService.GetTaskMemberListById(projectId);
            var list=users.Select(tm=> new TeamUserDto
            {   Id = tm.UserId,
                    Name =$"{tm.User.Firstname} {tm.User.Lastname}" ,
                    Phone=tm.User.PhoneNumber,
                    Occupation=tm.User.Occupation,
                    Email=tm.User.Email,
                    Photo=tm.User.Image,IsOnline=tm.User.IsOnline,

            }).ToList();
         
            return Ok(list);
        }
 

        [Authorize]
        [HttpPost("TeamMemberCollections")]///Sevgi
        public async Task<IActionResult> AddTeamMembersAsTeam([FromBody] TeamMemberCollectionDto dto)
        {
            var senderId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sender = await _userService.GetUserById(senderId);
            if (dto == null || dto.Members == null || !dto.Members.Any() )
            {
                return Ok(new {Message="No member"});
            }

            try
            {

                foreach (var username in dto.Members)
                {
                    var user = await _userService.GetOneUSerByUsername(username);
                    //var isCheck = await _teamMemberService.GetTaskMemberById(user.Id);
                    if (user == null)
                    {
                        return NotFound(new { Message = $"User '{username}' not found." });
                    }

                    //var teamMember = new TeamMember
                    //{
                    //    ProjectId = dto.ProjectId,
                    //    UserId = user.Id,
                    //};

                    //await _teamMemberService.Add(teamMember);
                    var project =await _projectService.GetProjectNameById(dto.ProjectId);

                    var request = new RequestNotification
                    {
                        IsAccepted = false,
                        ReceiverId = user.Id,
                        SenderId = senderId,
                        NotificationType = "ProjectRequest",
                        ProjectName =project,
                        SentDate = DateTime.UtcNow,
                        Text = "Hi, I am "+sender.Firstname+" "+sender.Lastname+ ". I want to invite you to my project named: " + project,
                    };
                    await _requestNotificationService.Add(request);
                    await _hub.Clients.User(user.Id).SendAsync("RequestList");
                    await _hub.Clients.User(user.Id).SendAsync("RequestList2");
                    await _hub.Clients.User(user.Id).SendAsync("RequestCount");
                    mailService.SendEmail(user.Email, sender.Firstname + "" + sender.Lastname + " invited you to their project " + project);
                    ///signalr
                }

                return Ok(new { Message = "Team members added successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while adding team members.", Details = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("TeammemberRemover")]
        public async Task<IActionResult> RemoveMember([FromBody] RemoveTeamMemberDto dto)
        {
           var requests= await _requestNotificationService.GetNotificationsByProjectName(dto.Title);
            var request = requests.FirstOrDefault(n => n.ReceiverId == dto.RecieverId);
            if (request == null)
            {
                return Ok(new { Code = 404 });
            }
                
            await _requestNotificationService.Delete(request);

            return Ok(new { Code = 200 });
        }

        [Authorize]
        [HttpDelete("MemberRemove")]
        public async Task<IActionResult> RemoveTM([FromBody]RemoveMemberDto dto)
        {
            var user=await _userService.GetOneUSerByUsername(dto.Username);
            var currentUserId=  HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _teamMemberService.DeleteTeamMemberAsync(dto.ProjectId,user.Id);
            var project = await _projectService.GetProjectById(dto.ProjectId);
            mailService.SendEmail(user.Email, "You were removed from project " + project.Title + " at " + DateTime.UtcNow.ToShortDateString() + " by PM");
            await _hub.Clients.User(currentUserId).SendAsync("ReceiveProjectUpdate");
            return Ok();

        }


        [Authorize]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetMembersByProjectId([FromRoute] int id)
        {
            var list = await _teamMemberService.GetTaskMemberListById(id);

            var dtoList = new List<ExtendedTeamMemberDto>();

            foreach (var item in list)
            {
                var user =await _userService.GetUserById(item.UserId);
                dtoList.Add(new ExtendedTeamMemberDto { Username = user.UserName, Firstname = user.Firstname,Lastname = user.Lastname, ImgPath = user.Image ,IsRequest=false, IsAccepted =true});

            }
            var project=await _projectService.GetProjectById(id);

            var requests = await _requestNotificationService.GetNotificationsByProjectName(project.Title);


            foreach (var item in requests)
            {
                var user = await _userService.GetUserById(item.ReceiverId);
                dtoList.Add(new ExtendedTeamMemberDto { Username = user.UserName, Firstname = user.Firstname, Lastname = user.Lastname, ImgPath = user.Image, IsRequest = true, IsAccepted=item.IsAccepted });

            }


            
                return Ok(new {List = dtoList});
            
            //return Ok(new {List = dtoList });
        }


    }
}
