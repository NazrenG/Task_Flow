using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.Business.Abstract;

namespace Task_Flow.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PremiumUserController : ControllerBase
    {
        private readonly IPremiumUserService _premiumUserService;

        public PremiumUserController(IPremiumUserService premiumUserService)
        {
            _premiumUserService = premiumUserService;
        }

        private string? CurrentUserId => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpPatch("UpgradeUserToPremiumPlan")]
        public async Task<IActionResult> UpgradeUserToPremiumPlan()
        {
            await _premiumUserService.UpgradeUserPlanToPremium(CurrentUserId!);
            return Ok();
        }

        [HttpPatch("UpgradeUserToBusinessPlan")]
        public async Task<IActionResult> UpgradeUserToBusinessPlan()
        {
            await _premiumUserService.UpgradeUserPlanToBusiness(CurrentUserId!);
            return Ok();
        }

        [HttpPatch("SwitchToFreePlan")]
        public async Task<IActionResult> SwitchToFreePlan()
        {
            await _premiumUserService.SwitchToFreePlan(CurrentUserId!);
            return Ok();
        }

        // GET api/<PremiumUserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PremiumUserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PremiumUserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PremiumUserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
