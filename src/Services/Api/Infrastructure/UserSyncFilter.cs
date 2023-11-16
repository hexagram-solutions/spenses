using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;
using Spenses.Application.Features.Users.Requests;

namespace Spenses.Api.Infrastructure;

public class UserSyncFilter(IMediator mediator) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await mediator.Send(new SyncCurrentUserCommand());

        await next();
    }
}
