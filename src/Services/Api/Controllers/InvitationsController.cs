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
    /// <param name="invitationId">The invitation identifier.</param>
    [HttpPatch("{invitationId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Patch))]
    // TODO: Can this be moved to the home controller? would need to put full link to home in email
    public async Task<ActionResult> AcceptInvitation(Guid invitationId)
    {
        await sender.Send(new AcceptInvitationCommand(invitationId));

        return NoContent();
    }

    /// <summary>
    /// Decline an invitation for the currently authenticated user.
    /// </summary>
    /// <param name="invitationId">The invitation identifier.</param>
    [HttpDelete("{invitationId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult> DeclineInvitation(Guid invitationId)
    {
        throw new NotImplementedException();
    }
}
