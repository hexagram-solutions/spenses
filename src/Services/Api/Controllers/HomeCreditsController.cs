using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Homes.Credits;
using Spenses.Application.Models;

namespace Spenses.Api.Controllers;

[ApiController]
[Route("/homes/{homeId:guid}/credits")]
public class HomeCreditsController : ApiControllerBase
{
    public HomeCreditsController(IMediator mediator)
        : base(mediator)
    {
    }

    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Credit>> PostCredit(Guid homeId, CreditProperties props)
    {
        return await GetCommandResult<Credit, CreateCreditCommand>(
            new CreateCreditCommand(homeId, props),
            x => CreatedAtAction(nameof(GetCredit), new { homeId, creditId = x.Id }, x));
    }

    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public async Task<ActionResult<IEnumerable<Credit>>> GetCredits(Guid homeId)
    {
        return await GetCommandResult<IEnumerable<Credit>, CreditsQuery>(new CreditsQuery(homeId), Ok);
    }

    [HttpGet("{creditId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Credit>> GetCredit(Guid homeId, Guid creditId)
    {
        return await GetCommandResult<Credit, CreditQuery>(new CreditQuery(homeId, creditId), Ok);
    }

    [HttpPut("{creditId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public async Task<ActionResult<Credit>> PutCredit(Guid homeId, Guid creditId, CreditProperties props)
    {
        return await GetCommandResult<Credit, UpdateCreditCommand>(
            new UpdateCreditCommand(homeId, creditId, props), Ok);
    }

    [HttpDelete("{creditId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public Task<ActionResult> DeleteCredit(Guid homeId, Guid creditId)
    {
        return GetCommandResult(new DeleteCreditCommand(homeId, creditId), NoContent);
    }
}
