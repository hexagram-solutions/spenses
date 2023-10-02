using FluentValidation.Results;

namespace Spenses.Application.Exceptions;

public class InvalidRequestException : Exception
{
    public InvalidRequestException()
        : this("One or more validation failures have occurred.")
    {
    }

    public InvalidRequestException(string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public InvalidRequestException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}

public class ForbiddenException : Exception
{
}

public class ResourceNotFoundException : Exception
{

    public ResourceNotFoundException(string resourceIdentifier)
        : base($"Resource with identifier {resourceIdentifier} was not found")
    {
        ResourceIdentifier = resourceIdentifier;
    }

    public ResourceNotFoundException(Guid resourceIdentifier)
        : this(resourceIdentifier.ToString())
    {
    }

    public string ResourceIdentifier { get; }
}
