using Microsoft.AspNetCore.Mvc;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Controllers.Extensions
{
    public static class ServiceResultExtensions
    {
        public static IActionResult ToActionResult<T>(this ControllerBase controller, ServiceResult<T> result)
        {
            return result.Status switch
            {
                ServiceResultStatus.Success => controller.Ok(result.Value),
                ServiceResultStatus.BadRequest => controller.BadRequest(result.Error),
                ServiceResultStatus.NotFound => result.Error == null ? controller.NotFound() : controller.NotFound(result.Error),
                _ => controller.StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };
        }
    }
}
