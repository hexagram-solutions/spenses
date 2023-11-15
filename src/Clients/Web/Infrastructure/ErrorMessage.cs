using Hexagrams.Extensions.Common.Serialization;
using Refit;

namespace Spenses.Client.Web.Infrastructure;

public record ErrorMessage(string Title, string? Details = null, Dictionary<string, string[]>? Errors = null);

public static class RefitExtensions
{
    public static ErrorMessage ToErrorMessage(this ApiException refitException)
    {
        var problem = refitException.Content!.FromJson<ProblemDetails>()!;

        return new ErrorMessage(problem.Title!, problem.Detail, problem.Errors);
    }
}
