using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.DTOs;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Hubs;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    { 
        private readonly ICompanyService _companyService;
        private readonly ICompanyWorkerService _companyWorkerService;
        private readonly IUserService _userService;
        private readonly IHubContext<ConnectionHub> _hub;
        public CompanyController(ICompanyService companyService, ICompanyWorkerService companyWorkerService, IUserService userService,IHubContext<ConnectionHub>hub)
        {
            _companyService = companyService;
            _companyWorkerService = companyWorkerService;
            _userService = userService;
            _hub = hub; 
        }
        // GET: api/<CompantController>
        //[HttpGet]
        //public Task<IActionResult> Get()
        //{


        //}

        // GET api/<CompantController>/5
        [HttpGet("GetCompany")]
        public async Task<IActionResult> Get()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId==null) { return BadRequest(); }
            var user=await _userService.GetUserById(userId);
            var company = await _companyService.GetCompany(userId);
            if (company == null) { return Ok(new {state=false}); }
            company.OwnerEmail = user.Email;
            company.OwnerUsername = user.UserName;
            company.OwnerFullname=user.Firstname+" "+user.Lastname;
           return Ok(new {Company=company,state=true});
        }

        [HttpGet("GetSortedUsers/{companyId}")]
        public async Task<IActionResult> GetSortedUsers(int companyId)
        {
            var users = await _companyWorkerService.SelectedUsersForCompany(companyId);
            return Ok(new {Users=users});
        }
        
        [HttpGet("GetCompanyWorkers/{companyId}")]
        public async Task<IActionResult> GetCompanyWorkers(int companyId)
        {
            var users = await _companyWorkerService.GetAllCompanyWorkers(w=>w.CompanyId== companyId);
            var dtos = new List<GetCompanyWorkersDto>();
            foreach (var item in users)
            {
                var user = await _userService.GetUserById(item.UserId);
                dtos.Add(new GetCompanyWorkersDto { Id = item.Id, Fullname = user.Firstname + " " + user.Lastname, Username = user.UserName,Occupation=item.Occupation });
            }
            return Ok(new {Users=dtos});
        }

        [HttpGet("GetCompanyProjects/{companyId}")]
        public async Task<IActionResult> GetCompanyProjects(int companyId)
        {
            var users = await _companyService.GetCompanyProjects(companyId);
            //var dtos = new List<GetCompanyWorkersDto>();
            //foreach (var item in users)
            //{
            //    var user = await _userService.GetUserById(item.UserId);
            //    dtos.Add(new GetCompanyWorkersDto { Id = item.Id, Fullname = user.Firstname + " " + user.Lastname, Username = user.UserName, Occupation = item.Occupation });
            //}
            return Ok(new { Users = users });
        }

        [HttpGet("SearchWorkerByKey")]
        public async Task<IActionResult> SearchWorkerByKey(
    [FromQuery] string key)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(key))
                return BadRequest("Search key is required");
            var company = await _companyService.GetCompany(userId);
            var result = await _companyWorkerService.SearchWorkerByKey(key, company.CompanyId);

            return Ok(new {Users=result});
        }



        // POST api/<CompantController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCompanyDto value)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var company = new Company
            {
                Address = value.Address,
                OwnerId = userId,
                CreatedDate = DateTime.UtcNow,
                Email = value.Email,
                Name = value.Name,
            };
            await _companyService.CreateCompanyAsync(company);
            return Ok();
        }

        // PUT api/<CompantController>/5
        [HttpPut("UpdateCompany/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CreateCompanyDto value)
        {
            await _companyService.UpdateCompany(id,value.Name,value.Email,value.Address);
        return Ok();
        }

        [HttpPut("UpdateCompanyPayment")]
        public async Task<IActionResult> UpdateCompanyPayment()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _companyService.UserPaidForCompany(userId);
            var company = await _companyService.GetCompany(userId);
           var userIds= await _companyWorkerService.CompanyReopened(company.CompanyId);
            await _hub.Clients.Users(userIds).SendAsync("PlanDowngraded", 3);
            return Ok();
        }

        // DELETE api/<CompantController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _companyService.DeleteCompany(id);
            var userIds=await _companyWorkerService.CompanyDeleted(id);
            await _hub.Clients.Users(userIds).SendAsync("PlanDowngraded", 0);

            return Ok();
        }

        [HttpDelete("RemoveCompanyWorker/{id}")]
        public async Task<IActionResult> RemoveCompanyWorker(int id)
        {
            await _companyWorkerService.RemoveCompanyWorker(id);
            return Ok();
        }
    }
}
