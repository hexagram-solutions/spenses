using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Homes;
using Spenses.Application.Models;

namespace Spenses.Api.Controllers;

[ApiController]
[Route("homes")]
public class HomesController : ApiControllerBase
{
    public HomesController(IMediator mediator)
        : base(mediator)
    {
    }

    [HttpPost]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<Home>> PostHome(HomeProperties props)
    {
        var home = await Mediator.Send(new CreateHomeCommand(props));

        return CreatedAtAction(nameof(GetHome), new { homeId = home.Id }, home);
    }

    [HttpGet]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.GetAll))]
    public Task<ActionResult<IEnumerable<Home>>> GetHomes()
    {
        return GetCommandResult<IEnumerable<Home>, HomesQuery>(new HomesQuery(), Ok);
    }

    [HttpGet("{homeId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Home>> GetHome(Guid homeId)
    {
        var home = await Mediator.Send(new HomeQuery(homeId));

        return Ok(home);
    }

    [HttpPut("{homeId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Put))]
    public Task<ActionResult<Home>> PutHome(Guid homeId, HomeProperties props)
    {
        return GetCommandResult<Home, UpdateHomeCommand>(new UpdateHomeCommand(homeId, props), Ok);
    }

    [HttpDelete("{homeId:guid}")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Delete))]
    public Task<ActionResult> DeleteHome(Guid homeId)
    {
        return GetCommandResult(new DeleteHomeCommand(homeId), NoContent);
    }
}
