using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("me")]
public class MeController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet("info")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<CurrentUser>> GetInfo()
    {
        var currentUser = await userManager.GetUserAsync(User);

        if (currentUser is null)
            return NotFound();

        return Ok(new CurrentUser
        {
            UserName = currentUser.UserName!,
            Email = currentUser.Email!,
            EmailVerified = currentUser.EmailConfirmed
        });
    }

    // post info

    // manage 2fa
}