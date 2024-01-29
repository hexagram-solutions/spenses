using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Members.Requests;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Models.Members;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/homes/{homeId:guid}/members")]
public class MembersController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Add a new member to a home.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="props">The member properties.</param>
    /// <returns>The new home member.</returns>
    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Member>> PostMember(Guid homeId, CreateMemberProperties props)
    {
        var member = await sender.Send(new CreateMemberCommand(homeId, props));

        return CreatedAtAction(nameof(GetMember), new { homeId, memberId = member.Id }, member);
    }

    /// <summary>
    /// Fetch the members of a home.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <returns>The home members.</returns>
    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<IEnumerable<Member>>> GetMembers(Guid homeId)
    {
        var members = await sender.Send(new MembersQuery(homeId));

        return Ok(members);
    }

    /// <summary>
    /// Fetch a home member.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="memberId">the member identifier.</param>
    /// <returns>The home member.</returns>
    [HttpGet("{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Member>> GetMember(Guid homeId, Guid memberId)
    {
        var member = await sender.Send(new MemberQuery(homeId, memberId));

        return Ok(member);
    }

    /// <summary>
    /// Update a home member.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="memberId">the member identifier.</param>
    /// <param name="props">The new member properties.</param>
    /// <returns>The updated member.</returns>
    [HttpPut("{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Member>> PutMember(Guid homeId, Guid memberId, MemberProperties props)
    {
        var member = await sender.Send(new UpdateMemberCommand(homeId, memberId, props));

        return Ok(member);
    }

    /// <summary>
    /// Attempt to remove a member from a home. If the member has anything associated with them, they will be
    /// deactivated instead.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="memberId">the member identifier.</param>
    /// <returns>The result of the operation.</returns>
    [HttpDelete("{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult<DeletionResult<Member>>> DeleteMember(Guid homeId, Guid memberId)
    {
        var result = await sender.Send(new DeleteMemberCommand(homeId, memberId));

        return Ok(result);
    }

    /// <summary>
    /// Re-activate an inactive member.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="memberId">the member identifier.</param>
    /// <returns>The activated member.</returns>
    [HttpPatch("{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Patch))]
    public async Task<ActionResult<Member>> ActivateMember(Guid homeId, Guid memberId)
    {
        var result = await sender.Send(new ActivateMemberCommand(homeId, memberId));

        return Ok(result);
    }

    /// <summary>
    /// Invite a user to join a home as the specified member.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="memberId">The member identifier.</param>
    /// <param name="props">The invitation properties.</param>
    /// <returns>The created invitation.</returns>
    [HttpPost("{memberId:guid}/invitations")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Invitation>> PostInvitation(Guid homeId, Guid memberId, InvitationProperties props)
    {
        var invitation = await sender.Send(new InviteExistingMemberCommand(homeId, memberId, props));

        return CreatedAtAction(nameof(GetInvitation), new
        {
            homeId,
            memberId,
            invitationid = invitation.Id
        }, invitation);
    }

    /// <summary>
    /// Fetch a member's invitations
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="memberId">The member identifier.</param>
    /// <returns>The invitation.</returns>
    [HttpGet("{memberId:guid}/invitations")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<Invitation[]>> GetInvitations(Guid homeId, Guid memberId)
    {
        var invitations = await sender.Send(new MemberInvitationsQuery(homeId, memberId));

        return Ok(invitations);
    }

    /// <summary>
    /// Fetch a member invitation.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="memberId">The member identifier.</param>
    /// <param name="invitationId">The invitation identifier.</param>
    /// <returns>The invitation.</returns>
    [HttpGet("{memberId:guid}/invitations/{invitationId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Invitation>> GetInvitation(Guid homeId, Guid memberId, Guid invitationId)
    {
        var invitation = await sender.Send(new MemberInvitationQuery(homeId, memberId, invitationId));

        return Ok(invitation);
    }

    /// <summary>
    /// Cancel a pending member invitation.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="memberId">The member identifier.</param>
    /// <param name="invitationId">The invitation identifier.</param>
    [HttpDelete("{memberId:guid}/invitations/{invitationId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult<Invitation>> CancelInvitation(Guid homeId, Guid memberId, Guid invitationId)
    {
        var invitation = await sender.Send(new CancelInvitationCommand(homeId, memberId, invitationId));

        return Ok(invitation);
    }
}
