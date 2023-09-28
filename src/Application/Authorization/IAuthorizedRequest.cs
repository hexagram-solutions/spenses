using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Spenses.Application.Authorization;

public interface IAuthorizedRequest<out TResponse> : IRequest<TResponse>
{
    AuthorizationPolicy Policy { get; }

    TResponse OnUnauthorized();
}
