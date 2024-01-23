using System.Diagnostics;
using Hexagrams.Extensions.Common.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Spenses.Application.Exceptions;

namespace Spenses.Api.Infrastructure;

public class ApplicationExceptionFilter : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        var exception = context.Exception;
        var type = context.Exception.GetType();

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
            return Task.CompletedTask;

        var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

        problemDetails.Extensions.Add("traceId", traceId);

        context.Result = new ContentResult
        {
            ContentType = "application/problem+json",
            StatusCode = problemDetails.Status,
            Content = problemDetails.ToJson()
        };

        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }
}
