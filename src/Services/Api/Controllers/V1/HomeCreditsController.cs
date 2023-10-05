using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Credits.Requests;
using Spenses.Application.Models.Credits;

namespace Spenses.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("/homes/{homeId:guid}/credits")]
public class HomeCreditsController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomeCreditsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Credit>> PostCredit(Guid homeId, CreditProperties props)
    {
        var credit = await _mediator.Send(new CreateCreditCommand(homeId, props));

        return CreatedAtAction(nameof(GetCredit), new { homeId, creditId = credit.Id }, credit);
    }

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

    [HttpGet("{creditId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Credit>> GetCredit(Guid homeId, Guid creditId)
    {
        var credit = await _mediator.Send(new CreditQuery(homeId, creditId));

        return Ok(credit);
    }

    [HttpPut("{creditId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Credit>> PutCredit(Guid homeId, Guid creditId, CreditProperties props)
    {
        var credit = await _mediator.Send(new UpdateCreditCommand(homeId, creditId, props));

        return Ok(credit);
    }

    [HttpDelete("{creditId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public async Task<ActionResult> DeleteCredit(Guid homeId, Guid creditId)
    {
        await _mediator.Send(new DeleteCreditCommand(homeId, creditId));

        return NoContent();
    }
}
