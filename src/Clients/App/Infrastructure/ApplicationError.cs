using Hexagrams.Extensions.Common.Serialization;
using Refit;

namespace Spenses.App.Infrastructure;

public record ApplicationError(string Title, string? Details = null, Dictionary<string, string[]>? Errors = null);

public static class RefitExtensions
{
    public static ApplicationError ToApplicationError(this ApiException refitException)
    {
        var problem = refitException.Content!.FromJson<ProblemDetails>()!;

        return problem.ToApplicationError();
    }

    public static ApplicationError ToApplicationError(this ProblemDetails problem)
    {
        return new ApplicationError(problem.Title!, problem.Detail, problem.Errors);
    }
}
