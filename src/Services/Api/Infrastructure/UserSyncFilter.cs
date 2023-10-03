using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;
using Spenses.Application.Features.Users.Requests;

namespace Spenses.Api.Infrastructure;

public class UserSyncFilter : IAsyncActionFilter
{
    private readonly IMediator _mediator;

    public UserSyncFilter(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await _mediator.Send(new SyncCurrentUserCommand());

        await next();
    }
}
