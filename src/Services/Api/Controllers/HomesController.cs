using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Homes.Requests;
using Spenses.Application.Models.Homes;

namespace Spenses.Api.Controllers;

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

    /// <summary>
    /// Create a new home.
    /// </summary>
    /// <param name="props">The home properties.</param>
    /// <returns>The new home.</returns>
    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Home>> PostHome(HomeProperties props)
    {
        var home = await _mediator.Send(new CreateHomeCommand(props));

        return CreatedAtAction(nameof(GetHome), new { homeId = home.Id }, home);
    }

    /// <summary>
    /// Fetch homes for the current user.
    /// </summary>
    /// <returns>The current user's homes.</returns>
    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<IEnumerable<Home>>> GetHomes()
    {
        var homes = await _mediator.Send(new HomesQuery());

        return Ok(homes);
    }

    /// <summary>
    /// Fetch information about a home.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <returns>The home information.</returns>
    [HttpGet("{homeId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Home>> GetHome(Guid homeId)
    {
        var home = await _mediator.Send(new HomeQuery(homeId));

        return Ok(home);
    }

    /// <summary>
    /// Update a home.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="props">The new home properties.</param>
    /// <returns>The updated home.</returns>
    [HttpPut("{homeId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Home>> PutHome(Guid homeId, HomeProperties props)
    {
        var home = await _mediator.Send(new UpdateHomeCommand(homeId, props));

        return Ok(home);
    }

    /// <summary>
    /// Delete a home.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    [HttpDelete("{homeId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult> DeleteHome(Guid homeId)
    {
        await _mediator.Send(new DeleteHomeCommand(homeId));

        return NoContent();
    }

    /// <summary>
    /// Fetch a summary of expenses and credits for the home.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="periodStart">The minimum date of credits and expenses to include in the breakdown.</param>
    /// <param name="periodEnd">The maximum date of credits and expenses to include in the breakdown.</param>
    /// <returns>The expense and credit balance breakdown.</returns>
    [HttpGet("{homeId:guid}/balance-breakdown")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<BalanceBreakdown>> GetBalanceBreakdown(Guid homeId, DateOnly periodStart,
        DateOnly periodEnd)
    {
        var balance = await _mediator.Send(new BalanceBreakdownQuery(homeId, periodStart, periodEnd));

        return Ok(balance);
    }
}
