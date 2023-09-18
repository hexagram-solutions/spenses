using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Common.Results;
using Spenses.Application.Homes;
using Spenses.Application.Homes.Members;
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
            CreatedAtAction(nameof(GetHome), new { id = result.Value.Id }, result.Value);
    }

    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<Home[]>> GetHomes()
    {
        var result = await _mediator.Send(new HomesQuery());

        return Ok(result.Value.ToArray());
    }

    [HttpGet("{id:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Home>> GetHome(Guid id)
    {
        var result = await _mediator.Send(new HomeQuery(id));

        return !result.IsSuccess
            ? (result.Result as ErrorResult)!.ToActionResult()
            : Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Home>> PutHome(Guid id, HomeProperties props)
    {
        var result = await _mediator.Send(new UpdateHomeCommand(id, props));

        return !result.IsSuccess
            ? (result.Result as ErrorResult)!.ToActionResult()
            : Ok(result.Value);
    }
}
