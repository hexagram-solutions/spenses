using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Homes.Requests;
using Spenses.Application.Models.Homes;

namespace Spenses.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("homes")]
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
        var home = await _mediator.Send(new CreateHomeCommand(props));

        return CreatedAtAction(nameof(GetHome), new { homeId = home.Id }, home);
    }

    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<IEnumerable<Home>>> GetHomes()
    {
        var homes = await _mediator.Send(new HomesQuery());

        return Ok(homes);
    }

    [HttpGet("{homeId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Home>> GetHome(Guid homeId)
    {
        var home = await _mediator.Send(new HomeQuery(homeId));

        return Ok(home);
    }

    [HttpPut("{homeId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Home>> PutHome(Guid homeId, HomeProperties props)
    {
        var home = await _mediator.Send(new UpdateHomeCommand(homeId, props));

        return Ok(home);
    }

    [HttpDelete("{homeId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult> DeleteHome(Guid homeId)
    {
        await _mediator.Send(new DeleteHomeCommand(homeId));

        return NoContent();
    }

    [HttpGet("{homeId:guid}/balance-breakdown")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<BalanceBreakdown>> GetBalanceBreakdown(Guid homeId, DateOnly periodStart,
        DateOnly periodEnd)
    {
        var balance = await _mediator.Send(new BalanceBreakdownQuery(homeId, periodStart, periodEnd));

        return Ok(balance);
    }
}
