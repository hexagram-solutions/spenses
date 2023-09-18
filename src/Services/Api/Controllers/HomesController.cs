using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Common.Results;
using Spenses.Application.Features.Homes;
using Spenses.Application.Features.Homes.Members;
using Spenses.Application.Models;

namespace Spenses.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HomesController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Home>> PostHome(HomeProperties props)
    {
        var result = await _mediator.Send(new CreateHomeCommand(props));

        return !result.IsSuccess ?
            (result.Result as ErrorResult)!.ToActionResult() :
            CreatedAtAction(nameof(GetHome), new { homeId = result.Value.Id }, result.Value);
    }

    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<Home[]>> GetHomes()
    {
        var result = await _mediator.Send(new HomesQuery());

        return Ok(result.Value.ToArray());
    }

    [HttpGet("{homeId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Home>> GetHome(Guid homeId)
    {
        var result = await _mediator.Send(new HomeQuery(homeId));

        return !result.IsSuccess
            ? (result.Result as ErrorResult)!.ToActionResult()
            : Ok(result.Value);
    }

    [HttpPut("{homeId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Home>> PutHome(Guid homeId, HomeProperties props)
    {
        var result = await _mediator.Send(new UpdateHomeCommand(homeId, props));

        return !result.IsSuccess
            ? (result.Result as ErrorResult)!.ToActionResult()
            : Ok(result.Value);
    }

    [HttpPost("{homeId:guid}/members")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Member>> PostMember(Guid homeId, MemberProperties props)
    {
        return await GetCommandResult<Member, AddMemberToHomeCommand>(
            new AddMemberToHomeCommand(homeId, props),
            x => CreatedAtAction(nameof(GetHome), new { homeId, memberId = x.Id }, x));
    }

    [HttpGet("{homeId:guid}/members/{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Member>> GetMember(Guid homeId, Guid memberId)
    {
        return await GetCommandResult<Member, MemberQuery>(new MemberQuery(memberId), Ok);
    }

    [HttpPut("{homeId:guid}/members/{memberId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Member>> PutMember(Guid homeId, Guid memberId, MemberProperties props)
    {
        return await GetCommandResult<Member, UpdateMemberCommand>(new UpdateMemberCommand(memberId, props), Ok);
    }

    private async Task<ActionResult<TResult>> GetCommandResult<TResult, TRequest>(TRequest request,
        Func<TResult, ActionResult> successAction)
        where TRequest : IRequest<ServiceResult<TResult>>
    {
        var result = await _mediator.Send(request);

        return !result.IsSuccess
            ? (result.Result as ErrorResult)!.ToActionResult()
            : successAction(result.Value);
    }
}
