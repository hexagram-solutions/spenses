using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Shared.Models.Invitations;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/homes/{homeId:guid}/invitations")]
public class InvitationsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Invite a new user to a home.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="props">The invitation properties.</param>
    /// <returns>The created invitation.</returns>
    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Invitation>> PostInvitation(Guid homeId, InvitationProperties props)
    {
        var invitation = new Invitation();

        return CreatedAtAction(nameof(GetInvitation), new { homeId, invitation.Id }, invitation);
    }

    /// <summary>
    /// Fetch all pending invitations for a home.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <returns>The invitations.</returns>
    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<Invitation[]>> GetInvitations(Guid homeId)
    {
        var invitations = new[] { new Invitation() };

        return Ok(invitations);
    }

    /// <summary>
    /// Fetch an invitation.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="invitationId">The invitation identifier.</param>
    /// <returns>The invitation.</returns>
    [HttpGet("{invitationId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Invitation>> GetInvitation(Guid homeId, Guid invitationId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Accept an invitation for the currently authenticated user.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="invitationId">The invitation identifier.</param>
    [HttpPatch("{invitationId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Patch))]
    public async Task<ActionResult<Invitation>> AcceptInvitation(Guid homeId, Guid invitationId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancel a pending invitation.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="invitationId">The invitation identifier.</param>
    [HttpDelete("{invitationId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult> CancelInvitation(Guid homeId, Guid invitationId)
    {
        return NoContent();
    }

}
