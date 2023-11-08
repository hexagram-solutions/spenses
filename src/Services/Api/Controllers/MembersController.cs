using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Members.Requests;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Members;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/homes/{homeId:guid}/members")]
public class MembersController : ControllerBase
{
    private readonly IMediator _mediator;

    public MembersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Add a new member to a home.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="props">The member properties.</param>
    /// <returns>The new home member.</returns>
    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Member>> PostMember(Guid homeId, MemberProperties props)
    {
        var member = await _mediator.Send(new CreateMemberCommand(homeId, props));

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
        var members = await _mediator.Send(new MembersQuery(homeId));

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
        var member = await _mediator.Send(new MemberQuery(homeId, memberId));

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
        var member = await _mediator.Send(new UpdateMemberCommand(homeId, memberId, props));

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
        var result = await _mediator.Send(new DeleteMemberCommand(homeId, memberId));

        return Ok(result);
    }

    /// <summary>
    /// Re-activate an inactive member.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="memberId">the member identifier.</param>
    /// <returns>The activated member.</returns>
    [HttpPut("{memberId:guid}/activate")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Member>> ActivateMember(Guid homeId, Guid memberId)
    {
        var result = await _mediator.Send(new ActivateMemberCommand(homeId, memberId));

        return Ok(result);
    }
}
