using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.WebAPI.Controllers.Extensions;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Companies;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyQueryService _companyQueryService;
        private readonly ICompanyCommandService _companyCommandService;

        public CompanyController(ICompanyQueryService companyQueryService, ICompanyCommandService companyCommandService)
        {
            _companyQueryService = companyQueryService;
            _companyCommandService = companyCommandService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet("GetCompany")]
        public async Task<IActionResult> Get()
        {
            var userId = CurrentUserId;
            if (userId == null) return BadRequest();

            return this.ToActionResult(await _companyQueryService.GetOwnCompanyAsync(userId));
        }

        [HttpGet("GetSortedUsers/{companyId}")]
        public async Task<IActionResult> GetSortedUsers(int companyId)
        {
            return this.ToActionResult(await _companyQueryService.GetSelectableUsersAsync(companyId));
        }

        [HttpGet("GetCompanyWorkers/{companyId}")]
        public async Task<IActionResult> GetCompanyWorkers(int companyId)
        {
            return this.ToActionResult(await _companyQueryService.GetWorkersAsync(companyId));
        }

        [HttpGet("GetCompanyProjects/{companyId}")]
        public async Task<IActionResult> GetCompanyProjects(int companyId)
        {
            return this.ToActionResult(await _companyQueryService.GetProjectsAsync(companyId));
        }

        [HttpGet("SearchWorkerByKey")]
        public async Task<IActionResult> SearchWorkerByKey([FromQuery] string key)
        {
            return this.ToActionResult(await _companyQueryService.SearchWorkersAsync(CurrentUserId, key));
        }

        // POST api/<CompanyController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCompanyDto value)
        {
            return this.ToActionResult(await _companyCommandService.CreateAsync(CurrentUserId, value));
        }

        // PUT api/<CompanyController>/5
        [HttpPut("UpdateCompany/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CreateCompanyDto value)
        {
            return this.ToActionResult(await _companyCommandService.UpdateAsync(id, value));
        }

        [HttpPut("UpdateCompanyPayment")]
        public async Task<IActionResult> UpdateCompanyPayment()
        {
            return this.ToActionResult(await _companyCommandService.MarkAsPaidAsync(CurrentUserId));
        }

        // DELETE api/<CompanyController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return this.ToActionResult(await _companyCommandService.DeleteAsync(id));
        }

        [HttpDelete("RemoveCompanyWorker/{id}")]
        public async Task<IActionResult> RemoveCompanyWorker(int id)
        {
            return this.ToActionResult(await _companyCommandService.RemoveWorkerAsync(id));
        }
    }
}
