using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Invitations.Requests;
using Spenses.Shared.Models.Invitations;
using Spenses.Utilities.Security;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("invitations/{invitationId:guid}")]
public class InvitationsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Accept an invitation for the currently authenticated user.
    /// </summary>
    /// <param name="invitationId">The invitation identifier.</param>
    /// <returns>The accepted invitation.</returns>
    [HttpPatch]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Patch))]
    public async Task<ActionResult<Invitation>> AcceptInvitation(Guid invitationId)
    {
        var invitation = await sender.Send(new AcceptInvitationCommand(invitationId, User.GetId()));

        return Ok(invitation);
    }

    /// <summary>
    /// Decline an invitation for the currently authenticated user.
    /// </summary>
    /// <param name="invitationId">The invitation identifier.</param>
    [HttpDelete]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult> DeclineInvitation(Guid invitationId)
    {
        throw new NotImplementedException();
    }
}
