using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;

namespace Spenses.Application.Common.Behaviors;

public class RequestPerformanceLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;
    private readonly ICurrentUserService _currentUserService;

    private readonly int _longRunningRequestThreshold;
    private readonly Stopwatch _timer;

    public RequestPerformanceLoggingBehavior(ILogger<TRequest> logger, ICurrentUserService currentUserService,
        IConfiguration configuration)
    {
        _logger = logger;
        _currentUserService = currentUserService;

        _longRunningRequestThreshold =
            int.Parse(configuration[ConfigConstants.SpensesLoggingLongRunningRequestThreshold]!);

        _timer = new Stopwatch();
        
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds <= _longRunningRequestThreshold)
            return response;

        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.CurrentUser.GetId();

        _logger.LogWarning("Request {Name} took {ElapsedMilliseconds}ms to complete for user {@UserId}",
            requestName, elapsedMilliseconds, userId);

        return response;
    }
}
