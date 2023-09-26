using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Homes.Members;
using Spenses.Application.Models;

namespace Spenses.Api.Controllers;

[ApiController]
[Route("/homes/{homeId:guid}/members")]
public class HomeMembersController : ApiControllerBase
{
    public HomeMembersController(IMediator mediator)
        : base(mediator)
    {
    }

    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public Task<ActionResult<Member>> PostMember(Guid homeId, MemberProperties props)
    {
        return GetCommandResult<Member, AddMemberToHomeCommand>(
            new AddMemberToHomeCommand(homeId, props),
            x => CreatedAtAction(nameof(GetMember), new { homeId, memberId = x.Id }, x));
    }

    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public Task<ActionResult<IEnumerable<Member>>> GetMembers(Guid homeId)
    {
        return GetCommandResult<IEnumerable<Member>, MembersQuery>(new MembersQuery(homeId), Ok);
    }

    [HttpGet("{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public Task<ActionResult<Member>> GetMember(Guid homeId, Guid memberId)
    {
        return GetCommandResult<Member, MemberQuery>(new MemberQuery(homeId, memberId), Ok);
    }

    [HttpPut("{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public Task<ActionResult<Member>> PutMember(Guid homeId, Guid memberId, MemberProperties props)
    {
        return GetCommandResult<Member, UpdateMemberCommand>(new UpdateMemberCommand(homeId, memberId, props), Ok);
    }

    [HttpDelete("{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public Task<ActionResult> DeleteMember(Guid homeId, Guid memberId)
    {
        return GetCommandResult(new DeleteMemberCommand(homeId, memberId), NoContent);
    }
}
