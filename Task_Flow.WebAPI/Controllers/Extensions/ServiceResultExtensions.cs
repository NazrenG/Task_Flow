using Microsoft.AspNetCore.Mvc;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Controllers.Extensions
{
    public static class ServiceResultExtensions
    {
        public static IActionResult ToActionResult<T>(this ControllerBase controller, ServiceResult<T> result)
        {
            switch (result.Status)
            {
                case ServiceResultStatus.Success:
                    if (result.Value is Empty) return controller.Ok();
                    return controller.Ok(result.Value);

                case ServiceResultStatus.BadRequest:
                    return controller.BadRequest(result.Error);

                case ServiceResultStatus.NotFound:
                    if (result.Error == null) return controller.NotFound();
                    return controller.NotFound(result.Error);

                case ServiceResultStatus.Unauthorized:
                    if (result.Error == null) return controller.Unauthorized();
                    return controller.Unauthorized(result.Error);

                default:
                    return controller.StatusCode(StatusCodes.Status500InternalServerError, result.Error);
            }
        }
    }
}
