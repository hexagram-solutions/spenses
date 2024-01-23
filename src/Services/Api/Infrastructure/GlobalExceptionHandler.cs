using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Spenses.Application.Exceptions;

namespace Spenses.Api.Infrastructure;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var type = exception.GetType();

        var problemDetails = exception switch
        {
            _ when type == typeof(InvalidRequestException) => new HttpValidationProblemDetails(
                ((InvalidRequestException) exception).Errors)
            {
                Title = "Bad Request",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            },
            _ when type == typeof(UnauthorizedException) => new ProblemDetails
            {
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2"
            },
            _ when type == typeof(ForbiddenException) => new ProblemDetails
            {
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4"
            },
            _ when type == typeof(ResourceNotFoundException) => new ProblemDetails
            {
                Title = "The specified resource was not found.",
                Detail = ((ResourceNotFoundException) exception).ResourceIdentifier,
                Status = StatusCodes.Status404NotFound,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5"
            },
            _ => null
        };

        if (problemDetails is null)
            return false;

        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        problemDetails.Extensions.Add("traceId", traceId);

        httpContext.Response.StatusCode =
            problemDetails.Status.GetValueOrDefault(StatusCodes.Status500InternalServerError);

        if (problemDetails is HttpValidationProblemDetails validationProblem)
            await httpContext.Response.WriteAsJsonAsync(validationProblem, cancellationToken);
        else
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);



        return true;
    }
}
