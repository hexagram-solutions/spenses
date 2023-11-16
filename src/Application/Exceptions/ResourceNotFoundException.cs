namespace Spenses.Application.Exceptions;

public class ResourceNotFoundException(string resourceIdentifier)
    : Exception($"Resource with identifier {resourceIdentifier} was not found")
{
    public ResourceNotFoundException(Guid resourceIdentifier)
        : this(resourceIdentifier.ToString())
    {
    }

    public string ResourceIdentifier { get; } = resourceIdentifier;
}
