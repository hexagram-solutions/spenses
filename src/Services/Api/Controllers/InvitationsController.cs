using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Invitations.Requests;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("invitations")]
public class InvitationsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Accept an invitation for the currently authenticated user.
    /// </summary>
    /// <param name="token">The invitation token from the email sent to the user.</param>
    [HttpPatch]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Patch))]
    public async Task<ActionResult> AcceptInvitation(string token)
    {
        await sender.Send(new AcceptInvitationCommand(token));

        return NoContent();
    }
}
