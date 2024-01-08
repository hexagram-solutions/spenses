using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Homes.Requests;
using Spenses.Shared.Models.Homes;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("homes")]
public class HomesController(IMediator mediator) : ControllerBase
{

    /// <summary>
    /// Create a new home.
    /// </summary>
    /// <param name="props">The home properties.</param>
    /// <returns>The new home.</returns>
    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Home>> PostHome(HomeProperties props)
    {
        var home = await mediator.Send(new CreateHomeCommand(props));

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
        var homes = await mediator.Send(new HomesQuery());

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
        var home = await mediator.Send(new HomeQuery(homeId));

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
        var home = await mediator.Send(new UpdateHomeCommand(homeId, props));

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
        await mediator.Send(new DeleteHomeCommand(homeId));

        return NoContent();
    }

    /// <summary>
    /// Fetch a summary of expenses and payments for the home.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="periodStart">The minimum date of payments and expenses to include in the breakdown.</param>
    /// <param name="periodEnd">The maximum date of payments and expenses to include in the breakdown.</param>
    /// <returns>The expense and payment balance breakdown.</returns>
    [HttpGet("{homeId:guid}/balance-breakdown")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<BalanceBreakdown>> GetBalanceBreakdown(Guid homeId, DateOnly periodStart,
        DateOnly periodEnd)
    {
        var balance = await mediator.Send(new BalanceBreakdownQuery(homeId, periodStart, periodEnd));

        return Ok(balance);
    }
}
