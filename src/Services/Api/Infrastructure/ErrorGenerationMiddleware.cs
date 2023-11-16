using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Spenses.Application.Common;

namespace Spenses.Api.Infrastructure;

public class ErrorGenerationMiddleware(RequestDelegate next, IConfiguration configuration)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var frequency = configuration.Require<double>(ConfigConstants.SpensesFeaturesErrorGenerationFrequency);

        var shouldError = Random.Shared.NextDouble() <= frequency;

        if (shouldError)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "OOPSIE WOOPSIE!!",
                Detail = "UwU We made a fucky wucky!! A wittle fucko boingo! The code monkeys at " +
                    "our headquarters are working VEWY HAWD to fix this!",
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            });

            return;
        }

        await next(context);
    }
}
