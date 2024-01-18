using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Insights.Requests;
using Spenses.Shared.Models.Insights;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/homes/{homeId:guid}/insights")]
public class InsightsController(IMediator mediator) : ControllerBase
{
    [HttpGet("expenses-over-time")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<IEnumerable<ExpenseTotalItem>>> GetExpensesOverTime(Guid homeId,
        ExpenseDateGrouping period)
    {
        var data = await mediator.Send(new TotalExpensesOverTimeQuery(homeId, period));

        return Ok(data);
    }
}
