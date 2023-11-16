using MediatR;
using Microsoft.AspNetCore.Authorization;
using Spenses.Application.Exceptions;
using Spenses.Utilities.Security.Services;

namespace Spenses.Application.Common.Behaviors;

public interface IBaseAuthorizedRequest
{
    AuthorizationPolicy Policy { get; }
}

public interface IAuthorizedRequest : IRequest, IBaseAuthorizedRequest
{
}

public interface IAuthorizedRequest<out TResponse> : IRequest<TResponse>, IBaseAuthorizedRequest
{
}

public class RequestAuthorizationBehavior<TRequest, TResponse>(IAuthorizationService authorizationService,
        ICurrentUserService currentUserService)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseAuthorizedRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var shouldAuthorize = request.GetType().GetInterfaces()
            .Any(i => i == typeof(IBaseAuthorizedRequest));

        if (!shouldAuthorize)
            return await next();

        var authorizedRequest = request as IBaseAuthorizedRequest;

        var currentUser = currentUserService.CurrentUser;

        var authorizationResult = await authorizationService.AuthorizeAsync(currentUser, authorizedRequest.Policy);

        if (authorizationResult.Succeeded)
            return await next();

        throw new ForbiddenException();
    }
}
