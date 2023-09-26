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
    public async Task<ActionResult<Member>> PostMember(Guid homeId, MemberProperties props)
    {
        return await GetCommandResult<Member, AddMemberToHomeCommand>(
            new AddMemberToHomeCommand(homeId, props),
            x => CreatedAtAction(nameof(GetMember), new { homeId, memberId = x.Id }, x));
    }

    [HttpGet("{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Member>> GetMember(Guid homeId, Guid memberId)
    {
        return await GetCommandResult<Member, MemberQuery>(new MemberQuery(memberId), Ok);
    }

    [HttpPut("{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Member>> PutMember(Guid homeId, Guid memberId, MemberProperties props)
    {
        return await GetCommandResult<Member, UpdateMemberCommand>(new UpdateMemberCommand(memberId, props), Ok);
    }
}
