using MediatR;
using Microsoft.AspNetCore.Authorization;
using Spenses.Application.Exceptions;
using Spenses.Utilities.Security.Services;

namespace Spenses.Application.Common.Behaviors;

public interface IAuthorizedRequest<out TResponse> : IRequest<TResponse>
{
    AuthorizationPolicy Policy { get; }
}

public class RequestAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuthorizedRequest<TResponse>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserService _currentUserService;

    public RequestAuthorizationBehavior(IAuthorizationService authorizationService,
        ICurrentUserService currentUserService)
    {
        _authorizationService = authorizationService;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var shouldAuthorize = request.GetType().GetInterfaces()
            .Any(i => i == typeof(IAuthorizedRequest<TResponse>));

        if (!shouldAuthorize)
            return await next();

        var authorizedRequest = request as IAuthorizedRequest<TResponse>;

        var currentUser = _currentUserService.CurrentUser;

        var authorizationResult = await _authorizationService.AuthorizeAsync(currentUser, authorizedRequest.Policy);

        if (authorizationResult.Succeeded)
            return await next();

        throw new ForbiddenException();
    }
}
