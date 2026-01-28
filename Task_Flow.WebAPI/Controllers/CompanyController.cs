using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    { 
        private readonly ICompanyService _companyService;
        private readonly ICompanyWorkerService _companyWorkerService;
        public CompanyController(ICompanyService companyService, ICompanyWorkerService companyWorkerService)
        {
            _companyService = companyService;
            _companyWorkerService = companyWorkerService;
        }
        // GET: api/<CompantController>
        //[HttpGet]
        //public Task<IActionResult> Get()
        //{


        //}

        // GET api/<CompantController>/5
        [HttpGet("GetCompany/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id == 0) { return BadRequest(); }
           return Ok(new {company=await _companyService.GetCompany(id)});
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
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CompantController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
