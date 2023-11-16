using FluentValidation.Results;

namespace Spenses.Application.Exceptions;

public class InvalidRequestException(string message) : Exception(message)
{
    public InvalidRequestException()
        : this("One or more validation failures have occurred.")
    {
    }

    public InvalidRequestException(params ValidationFailure[] failures)
        : this(failures.AsEnumerable())
    {
    }

    public InvalidRequestException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();
}
