using System.Diagnostics;
using Hexagrams.Extensions.Configuration;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spenses.Shared.Common;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;

namespace Spenses.Application.Common.Behaviors;

public class RequestPerformanceLoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger,
    ICurrentUserService currentUserService, IConfiguration configuration)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly int _longRunningRequestThreshold =
            configuration.Require<int>(ConfigConstants.SpensesLoggingLongRunningRequestThreshold);

    private readonly Stopwatch _timer = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds <= _longRunningRequestThreshold)
            return response;

        var requestName = typeof(TRequest).Name;

        var currentUser = currentUserService.CurrentUser;

        if (currentUser.Identity?.IsAuthenticated == false)
        {
            logger.LogWarning(
                "Request {Name} took {ElapsedMilliseconds}ms to complete (threshold: {Threshold})",
                requestName, elapsedMilliseconds, _longRunningRequestThreshold);
        }

        var userId = currentUser.GetId();

        logger.LogWarning(
            "Request {Name} took {ElapsedMilliseconds}ms to complete for user {@UserId} (threshold: {Threshold})",
            requestName, elapsedMilliseconds, userId, _longRunningRequestThreshold);

        return response;
    }
}
