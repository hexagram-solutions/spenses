using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Members.Requests;
using Spenses.Application.Models.Members;

namespace Spenses.Api.Controllers;

[ApiController]
[Route("/homes/{homeId:guid}/members")]
public class HomeMembersController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomeMembersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Member>> PostMember(Guid homeId, MemberProperties props)
    {
        var member = await _mediator.Send(new CreateMemberCommand(homeId, props));

        return CreatedAtAction(nameof(GetMember), new { homeId, memberId = member.Id }, member);
    }

    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<IEnumerable<Member>>> GetMembers(Guid homeId)
    {
        var members = await _mediator.Send(new MembersQuery(homeId));

        return Ok(members);
    }

    [HttpGet("{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Member>> GetMember(Guid homeId, Guid memberId)
    {
        var member = await _mediator.Send(new MemberQuery(homeId, memberId));

        return Ok(member);
    }

    [HttpPut("{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Member>> PutMember(Guid homeId, Guid memberId, MemberProperties props)
    {
        var member = await _mediator.Send(new UpdateMemberCommand(homeId, memberId, props));

        return Ok(member);
    }

    [HttpDelete("{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult> DeleteMember(Guid homeId, Guid memberId)
    {
        await _mediator.Send(new DeleteMemberCommand(homeId, memberId));

        return NoContent();
    }
}
