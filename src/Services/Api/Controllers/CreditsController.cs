using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Credits.Requests;
using Spenses.Application.Models.Credits;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/homes/{homeId:guid}/credits")]
public class CreditsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreditsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new credit.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="props">The credit properties.</param>
    /// <returns>The newly created credit.</returns>
    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Credit>> PostCredit(Guid homeId, CreditProperties props)
    {
        var credit = await _mediator.Send(new CreateCreditCommand(homeId, props));

        return CreatedAtAction(nameof(GetCredit), new { homeId, creditId = credit.Id }, credit);
    }

    /// <summary>
    /// Query credits with various parameters.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="query">The parameters to query credits with.</param>
    /// <returns>The filtered and sorted credits.</returns>
    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<IEnumerable<CreditDigest>>> GetCredits(Guid homeId,
        [FromQuery] FilteredCreditsQuery query)
    {
        var credits = await _mediator.Send(new CreditsQuery(homeId)
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            OrderBy = query.OrderBy,
            SortDirection = query.SortDirection,
            MinDate = query.MinDate,
            MaxDate = query.MaxDate,
        });

        return Ok(credits);
    }

    /// <summary>
    /// Fetch a credit.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="creditId">The credit identifier.</param>
    /// <returns>The credit.</returns>
    [HttpGet("{creditId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Credit>> GetCredit(Guid homeId, Guid creditId)
    {
        var credit = await _mediator.Send(new CreditQuery(homeId, creditId));

        return Ok(credit);
    }

    /// <summary>
    /// Updates a credit.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="creditId">The credit identifier.</param>
    /// <param name="props">The new credit properties.</param>
    /// <returns></returns>
    [HttpPut("{creditId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Credit>> PutCredit(Guid homeId, Guid creditId, CreditProperties props)
    {
        var credit = await _mediator.Send(new UpdateCreditCommand(homeId, creditId, props));

        return Ok(credit);
    }

    /// <summary>
    /// Deletes a credit.
    /// </summary>
    /// <param name="homeId">The home identifier.</param>
    /// <param name="creditId">The credit identifier.</param>
    [HttpDelete("{creditId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult> DeleteCredit(Guid homeId, Guid creditId)
    {
        await _mediator.Send(new DeleteCreditCommand(homeId, creditId));

        return NoContent();
    }
}
