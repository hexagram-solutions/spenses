using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Me.Requests;
using Spenses.Shared.Models.Me;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("me")]
public class MeController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Get basic information about the currently authenticated user.
    /// </summary>
    /// <returns>Basic information about the current user.</returns>
    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<CurrentUser>> GetMe()
    {
        var currentUser = await sender.Send(new CurrentUserQuery());

        return Ok(currentUser);
    }

    /// <summary>
    /// Update the current user's profile information.
    /// </summary>
    /// <returns>The updated user's information.</returns>
    [HttpPut]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<CurrentUser>> UpdateMe(UserProfileProperties props)
    {
        var currentUser = await sender.Send(new UpdateCurrentUserCommand(props));

        return Ok(currentUser);
    }

    /// <summary>
    /// Request an email change verification message for the current user.
    /// </summary>
    [HttpPut("change-email")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult> ChangeEmail(ChangeEmailRequest request)
    {
        await sender.Send(new ChangeEmailCommand(request));

        return Ok();
    }

    /// <summary>
    /// Change the current user's password.
    /// </summary>
    [HttpPut("change-password")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult> ChangePassword()
    {
        throw new NotImplementedException();
    }

    [HttpPut("enable-2fa")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public Task<ActionResult> EnableTwoFactorLogin()
    {
        throw new NotImplementedException();
    }

    [HttpPut("disable-2fa")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public Task<ActionResult> DisableTwoFactorLogin()
    {
        throw new NotImplementedException();
    }

    [HttpPut("regenerate-recovery-codes")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public Task<ActionResult> RegenerateRecoveryCodes()
    {
        throw new NotImplementedException();
    }
}
