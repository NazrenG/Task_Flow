using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
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
        public CompanyController(ICompanyService companyService, ICompanyWorkerService companyWorkerService, IUserService userService)
        {
            _companyService = companyService;
            _companyWorkerService = companyWorkerService;
            _userService = userService;
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
            return Ok();
        }

        // DELETE api/<CompantController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _companyService.DeleteCompany(id);
            return Ok();
        }
    }
}
