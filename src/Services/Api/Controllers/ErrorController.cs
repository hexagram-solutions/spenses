using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Spenses.Application.Common;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    [Route("/error-development")]
    public IActionResult ErrorLocalDevelopment(
        [FromServices] IWebHostEnvironment webHostEnvironment)
    {
        if (webHostEnvironment.IsEnvironment(EnvironmentNames.Local) ||
            webHostEnvironment.IsEnvironment(EnvironmentNames.IntegrationTest) ||
            webHostEnvironment.IsEnvironment(EnvironmentNames.Test))
        {
            throw new InvalidOperationException(
                "This shouldn't be invoked in non-local or non-test environments.");
        }

        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

        return Problem(
            detail: context?.Error.StackTrace,
            title: context?.Error.Message);
    }

    [Route("/error")]
    public IActionResult Error() => Problem();
}
