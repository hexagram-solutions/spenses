using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    [Route("/error-development")]
    public IActionResult ErrorLocalDevelopment()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

        return Problem(
            detail: context?.Error.StackTrace,
            title: context?.Error.Message);
    }

    [Route("/error")]
    public IActionResult Error() => Problem();
}
