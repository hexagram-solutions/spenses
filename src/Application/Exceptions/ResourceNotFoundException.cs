namespace Spenses.Application.Exceptions;

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
