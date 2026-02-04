using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.Business.Abstract;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        [HttpPatch("UpgradeUserToPremiumPlan")]
        public async Task<IActionResult> UpgradeUserToPremiumPlan()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _premiumUserService.UpgradeUserPlanToPremium(userId);
            return Ok();
        }
        [HttpPatch("UpgradeUserToBusinessPlan")]
        public async Task<IActionResult> UpgradeUserToBusinessPlan()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _premiumUserService.UpgradeUserPlanToBusiness(userId);
            return Ok();
        } 
        [HttpPatch("SwitchToFreePlan")]
        public async Task<IActionResult> SwitchToFreePlan()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _premiumUserService.SwitchToFreePlan(userId);
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
