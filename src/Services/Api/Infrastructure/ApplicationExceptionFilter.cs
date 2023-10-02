using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Spenses.Application.Exceptions;

namespace Spenses.Api.Infrastructure;

public class ApplicationExceptionFilter : IAsyncExceptionFilter
{
    private readonly Dictionary<Type, Func<ExceptionContext, Task>> _exceptionHandlers;

    public ApplicationExceptionFilter()
    {
        _exceptionHandlers = new()
        {
            { typeof(InvalidRequestException), HandleValidationException }
        };
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        var exception = context.Exception;

        if (!_exceptionHandlers.TryGetValue(exception.GetType(), out var handle))
            return;

        await handle(context);

        context.ExceptionHandled = true; // todo: correct? what about other exceptions?
    }

    private Task HandleValidationException(ExceptionContext context)
    {
        var exception = (InvalidRequestException) context.Exception;

        if (!exception.Errors.Any())
        {
            context.Result = new BadRequestObjectResult(new ProblemDetails
            {
                Title = "Bad request",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });

            return Task.CompletedTask;
        }

        var modelStateDictionary = new ModelStateDictionary();

        foreach (var (propertyName, errors) in exception.Errors)
        {
            foreach (var error in errors)
            {
                modelStateDictionary.AddModelError(propertyName, error);
            }
        }

        var validationProblemDetails = new ValidationProblemDetails(modelStateDictionary);

        context.Result = new BadRequestObjectResult(validationProblemDetails);

        return Task.CompletedTask;
    }
}
